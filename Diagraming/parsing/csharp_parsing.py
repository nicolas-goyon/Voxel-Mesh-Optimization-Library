# parsing/csharp_parsing.py
import os
import re
import regex  # requires: pip install regex
from data.project import Project, convert_parsed_type

# -----------------------------
# Module‑level regex patterns
# -----------------------------
# Comments
SINGLE_LINE_COMMENT_PATTERN = r'//.*?$'
MULTI_LINE_COMMENT_PATTERN = r'/\*.*?\*/'
REGION_DIRECTIVE_PATTERN = r'^\s*#(?:region|endregion).*$'

# Helpers
BRACE_PATTERN = r'\{(?:[^{}]+|(?R))*\}'

TYPE_DECL_PATTERN = (
    r'\b(?P<kind>class|interface|enum)\s+'
    r'(?P<name>\w+(?:<[^>]+>)?)'
    r'(?:\s+where\s+[^{]+)?'
    r'(?:\s*:\s*(?P<bases>[^<{]+))?'
)

# namespaces
FILE_SCOPED_NAMESPACE_PATTERN = r'^\s*namespace\s+([\w\.]+)\s*;'
BLOCK_SCOPED_NAMESPACE_PATTERN = r'\bnamespace\s+([\w\.]+)\s*\{'

# Methods
PUBLIC_METHOD_PATTERN = (
    r'^\s*'                                        # Anchor to the beginning of the line and skip leading whitespace
    r'public\s+'                                   # Match "public" with trailing whitespace
    r'(?P<modifier>(?:static\s+)?)'                 # Optional static modifier
    r'\s*(?P<return_type>[\w<>\[\],]+?)\s+'         # Return type (capturing common tokens without extra whitespace)
    r'(?P<name>\w+)\s*'                             # Method name
    r'\('
        r'(?P<params>(?:[^()]+|(?R))*)'             # Recursive capture of parameters
    r'\)'                                         
    r'\s*\{'                                      # Opening brace of method body (indicates the start of the body)
)
INTERFACE_METHOD_PATTERN = (
    r'^\s*'  # anchor at the start of a line and skip leading whitespace
    r'(?P<accessibility>(?:public|private|protected|internal)\s+)?'  # Optional accessibility
    r'(?P<modifier>(?:static\s+)?)'                                   # Optional static modifier
    r'\s*(?P<return_type>[\w<>\[\],]+?)\s+'                            # Return type 
    r'(?P<name>\w+)\s*'                                                # Method name
    r'\('
        r'(?P<params>(?:[^()]+|(?R))*)'
    r'\)\s*;'
)

# New regex pattern for properties.
# It matches property declarations that start with public (plus optional static),
# followed by the property type and name.
# The group "delim" matches either an opening brace '{' for a normal property
# or '=>' for an expression-bodied property.
PROPERTY_PATTERN = (
    r'^\s*public\s+'
    r'(?P<modifier>(?:\b(?:static|readonly)\b\s+)*)'
    r'(?P<type>[\w<>\[\]\(\),\s]+?)\s+'
    r'(?P<name>\w+)\s*'
    r'(?P<delim>\{|=>)'
)


# Other
ENUM_MEMBER_PATTERN = r'\b(\w+)\b(?:\s*=\s*[^,\n]+)?\s*(?:,|$)'

# -----------------------------
# Helper Functions
# -----------------------------
def remove_comments(text):
    text = re.sub(SINGLE_LINE_COMMENT_PATTERN, '', text, flags=re.MULTILINE)
    text = re.sub(MULTI_LINE_COMMENT_PATTERN, '', text, flags=re.DOTALL)
    text = re.sub(REGION_DIRECTIVE_PATTERN, '', text, flags=re.MULTILINE)
    return text

def find_matching_brace(text, start_index):
    if text[start_index] != '{':
        return -1
    pattern = regex.compile(BRACE_PATTERN)
    match = pattern.match(text, pos=start_index)
    if match:
        return match.end() - 1
    return -1

def extract_public_methods(class_block):
    methods = []
    pattern = regex.compile(PUBLIC_METHOD_PATTERN, flags=regex.MULTILINE)
    for match in pattern.finditer(class_block):
        modifier = match.group('modifier').strip()
        method_name = match.group('name')
        raw_params = match.group('params').strip().splitlines()
        clean_params = []
        for param in raw_params:
            clean_params.append(param.strip())
        clean_params = ' '.join(clean_params)
        methods.append({
            "name": method_name,
            "static": "static" in modifier,
            "params": clean_params,
            "return_type": match.group('return_type')
        })
    return methods

def extract_interface_methods(interface_block):
    methods = []
    pattern = regex.compile(INTERFACE_METHOD_PATTERN, flags=regex.MULTILINE)
    for match in pattern.finditer(interface_block):
        modifier = match.group('modifier').strip()
        method_name = match.group('name')
        raw_params = match.group('params').strip().splitlines()
        clean_params= []
        for param in raw_params:
            clean_params.append(param.strip())
        clean_params = ' '.join(clean_params)
        methods.append({
            "name": method_name,
            "static": "static" in modifier,
            "params": clean_params,
            "return_type": match.group('return_type')
        })
    return methods

def extract_enum_members(enum_block):
    content = enum_block.strip().rstrip("}").strip()
    members = re.findall(ENUM_MEMBER_PATTERN, content)
    return [m for m in members if m.strip()]

def extract_properties(block_text):
    """
    Extract property declarations from a class or interface block.
    Supports both block-based properties (with { get; set; } or computed getters/setters)
    and expression-bodied properties (using =>).
    """
    properties = []
    pattern = regex.compile(PROPERTY_PATTERN, flags=regex.MULTILINE)
    for match in pattern.finditer(block_text):
        modifier = match.group('modifier').strip()
        prop_type = match.group('type')
        prop_name = match.group('name')
        delim = match.group('delim')
        
        if delim == '=>':
            # Expression-bodied property: capture until the semicolon.
            start_index = match.end()
            end_index = block_text.find(';', start_index)
            if end_index == -1:
                body = block_text[start_index:].strip()
            else:
                body = block_text[start_index:end_index].strip()
        else:
            # Block-based property: use find_matching_brace to capture entire accessor block.
            open_brace_index = match.end() - 1  # the '{' char is included in the match.
            close_brace_index = find_matching_brace(block_text, open_brace_index)
            if close_brace_index == -1:
                body = ""
            else:
                body = block_text[open_brace_index:close_brace_index + 1]
        properties.append({
            "name": prop_name,
            "static": "static" in modifier,
            "type": prop_type,
            "body": body
        })
    return properties

def parse_types_from_text(text):
    types_list = []
    pos = 0
    decl_pattern = re.compile(TYPE_DECL_PATTERN)
    while pos < len(text):
        m = decl_pattern.search(text, pos)
        if not m:
            break
        typ = m.group('kind')
        name = m.group('name')
        bases = []
        if m.group('bases'):
            bases = [b.strip() for b in m.group('bases').split(",") if b.strip()]
        abs_decl_end = m.end()
        open_brace_index = text.find('{', abs_decl_end)
        if open_brace_index == -1:
            pos = abs_decl_end + 1
            continue
        closing_brace_index = find_matching_brace(text, open_brace_index)
        if closing_brace_index == -1:
            pos = abs_decl_end + 1
            continue
        block_text = text[open_brace_index + 1:closing_brace_index]
        pos = closing_brace_index + 1
        
        item = {
            "kind": typ,
            "name": name,
            "bases": bases,
            "namespace": None
        }
        # For classes and interfaces, extract both methods and properties.
        if typ == "class":
            item["methods"] = extract_public_methods(block_text)
            item["properties"] = extract_properties(block_text)
        elif typ == "interface":
            item["methods"] = extract_interface_methods(block_text)
            item["properties"] = extract_properties(block_text)
        elif typ == "enum":
            item["members"] = extract_enum_members(block_text)
        types_list.append(item)
    return types_list

def get_namespace_blocks(text):
    namespace_blocks = []
    fs_match = re.search(FILE_SCOPED_NAMESPACE_PATTERN, text, flags=re.MULTILINE)
    if fs_match:
        ns_name = fs_match.group(1).strip()
        start_index = fs_match.end()
        block_text = text[start_index:]
        namespace_blocks.append((ns_name, block_text))
        return namespace_blocks
    for match in re.finditer(BLOCK_SCOPED_NAMESPACE_PATTERN, text):
        ns_name = match.group(1).strip()
        start_brace = match.end()
        end_brace = find_matching_brace(text, start_brace-1)
        if end_brace != -1:
            block_text = text[start_brace:end_brace]
            namespace_blocks.append((ns_name, block_text))
    return namespace_blocks

def parse_cs_file(file_path, exclude_files=None):
    if exclude_files:
        for pattern in exclude_files:
            if pattern in file_path:
                return []
    try:
        with open(file_path, 'r', encoding="utf-8") as file:
            content = file.read()
    except Exception as e:
        print(f"Error reading {file_path}: {e}")
        return []
    content = remove_comments(content)
    ns_blocks = get_namespace_blocks(content)
    types_parsed = []
    if ns_blocks:
        for ns_name, block in ns_blocks:
            types_list = parse_types_from_text(block)
            for t in types_list:
                t["namespace"] = ns_name
            types_parsed.extend(types_list)
    else:
        types_list = parse_types_from_text(content)
        for t in types_list:
            t["namespace"] = "Global"
        types_parsed.extend(types_list)
    return types_parsed

# -----------------------------
# Parse Entire Project
# -----------------------------
def parse_project(folder_path, exclude_files=None):
    """
    Traverse folder_path, parse all C# files and return a Project object.
    """
    # First, get raw parsed types (dictionaries) using the existing logic.
    raw_types = []
    for root, _, files in os.walk(folder_path):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                raw_types.extend(parse_cs_file(file_path, exclude_files))
    
    # Create a Project object from the raw data.
    from data.project import Project, convert_parsed_type  # Import here if there are import issues with relative paths.
    project = Project()
    for raw in raw_types:
        project.add_type(convert_parsed_type(raw))
    return project
