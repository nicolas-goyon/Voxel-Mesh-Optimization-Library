# data/project.py
import re
from typing import List, Dict, Optional, Any

class Parameter:
    def __init__(self, param_type: str, name: str, annotations: Optional[List[str]] = None):
        self.param_type = param_type
        self.name = name
        self.annotations = annotations if annotations is not None else []

    def __repr__(self):
        return f"<Parameter {self.param_type} {self.name}>"

    def describe(self, indent: int = 0) -> str:
        pad = "  " * indent
        lines = [
            f"{pad}Parameter:",
            f"{pad}  Type       : {self.param_type}",
            f"{pad}  Name       : {self.name}",
            f"{pad}  Annotations: {self.annotations}"
        ]
        return "\n".join(lines)

class Method:
    def __init__(
        self,
        name: str,
        parameters: Optional[List[Parameter]] = None,
        return_type: Optional[str] = None,
        static: bool = False,
        annotations: Optional[List[str]] = None
    ):
        self.name = name
        self.parameters = parameters if parameters is not None else []
        self.return_type = return_type
        self.static = static
        self.annotations = annotations if annotations is not None else []

    def __repr__(self):
        ret = f": {self.return_type}" if self.return_type else ""
        static_str = " static" if self.static else ""
        params = ", ".join(f"{p.param_type} {p.name}" for p in self.parameters)
        return f"<Method{static_str} {self.name}({params}){ret}>"

    def describe(self, indent: int = 0) -> str:
        pad = "  " * indent
        header = f"{pad}Method:"
        static_str = "Yes" if self.static else "No"
        lines = [
            header,
            f"{pad}  Name       : {self.name}",
            f"{pad}  Static     : {static_str}",
            f"{pad}  Return Type: {self.return_type}",
            f"{pad}  Annotations: {self.annotations}",
            f"{pad}  Parameters :"
        ]
        if self.parameters:
            for param in self.parameters:
                lines.append(param.describe(indent + 2))
        else:
            lines.append(f"{pad}    None")
        return "\n".join(lines)

class Property:
    def __init__(
        self,
        name: str,
        property_type: str,
        body: str,
        static: bool = False,
        annotations: Optional[List[str]] = None
    ):
        self.name = name
        self.property_type = property_type
        self.body = body
        self.static = static
        self.annotations = annotations if annotations is not None else []

    def __repr__(self):
        static_str = " static" if self.static else ""
        return f"<Property{static_str} {self.property_type} {self.name}>"

    def describe(self, indent: int = 0) -> str:
        pad = "  " * indent
        static_str = "Yes" if self.static else "No"
        lines = [
            f"{pad}Property:",
            f"{pad}  Name       : {self.name}",
            f"{pad}  Type       : {self.property_type}",
            f"{pad}  Static     : {static_str}",
            f"{pad}  Body       : {self.body}",
            f"{pad}  Annotations: {self.annotations}"
        ]
        return "\n".join(lines)

class ParsedType:
    def __init__(
        self,
        kind: str,
        name: str,
        bases: List[str],
        methods: Optional[List[Method]] = None,
        members: Optional[List[str]] = None,
        properties: Optional[List[Any]] = None,
        annotations: Optional[List[str]] = None,
        generics: Optional[str] = None,
        namespace: Optional[str] = None
    ):
        self.kind = kind
        self.name = name
        self.bases = bases
        self.methods = methods if methods is not None else []
        self.members = members if members is not None else []
        self.properties = properties if properties is not None else []
        self.annotations = annotations if annotations is not None else []
        self.generics = generics
        self.namespace = namespace

    def __repr__(self):
        gen = f"<{self.generics}>" if self.generics else ""
        return f"<ParsedType {self.kind} {self.name}{gen} (ns: {self.namespace})>"

    def describe(self, indent: int = 0) -> str:
        pad = "  " * indent
        gen_str = self.generics if self.generics else "None"
        lines = [
            f"{pad}ParsedType:",
            f"{pad}  Kind       : {self.kind}",
            f"{pad}  Name       : {self.name}",
            f"{pad}  Generics   : {gen_str}",
            f"{pad}  Namespace  : {self.namespace}",
            f"{pad}  Bases      : {self.bases}",
            f"{pad}  Annotations: {self.annotations}",
            f"{pad}  Members    : {self.members}",
            f"{pad}  Properties :"
        ]
        if self.properties:
            for prop in self.properties:
                lines.append(prop.describe(indent + 2))
        else:
            lines.append(f"{pad}    None")
        lines.append(f"{pad}  Methods    :")
        if self.methods:
            for method in self.methods:
                lines.append(method.describe(indent + 2))
        else:
            lines.append(f"{pad}    None")
        return "\n".join(lines)

class Namespace:
    def __init__(self, name: str, full_name: Optional[str] = None):
        self.name = name
        self.full_name = full_name if full_name is not None else name
        self.types: List[ParsedType] = []
        self.sub_namespaces: Dict[str, 'Namespace'] = {}

    def add_type(self, parsed_type: ParsedType):
        self.types.append(parsed_type)

    def add_subnamespace(self, sub_ns: 'Namespace'):
        self.sub_namespaces[sub_ns.name] = sub_ns

    def __repr__(self):
        return f"<Namespace {self.full_name}: {len(self.types)} types, {len(self.sub_namespaces)} sub-namespaces>"

    def describe(self, indent: int = 0) -> str:
        pad = "  " * indent
        lines = [
            f"{pad}Namespace:",
            f"{pad}  Name        : {self.name}",
            f"{pad}  Full Name   : {self.full_name}",
            f"{pad}  Types       :"
        ]
        if self.types:
            for typ in self.types:
                lines.append(typ.describe(indent + 2))
        else:
            lines.append(f"{pad}    None")
        lines.append(f"{pad}  Sub-namespaces:")
        if self.sub_namespaces:
            for sub_ns in self.sub_namespaces.values():
                lines.append(sub_ns.describe(indent + 2))
        else:
            lines.append(f"{pad}    None")
        return "\n".join(lines)

class Project:
    def __init__(self):
        self.namespaces: Dict[str, Namespace] = {}
        self.global_types: List[ParsedType] = []

    def add_type(self, parsed_type: ParsedType):
        ns_name = parsed_type.namespace or "Global"
        if ns_name == "Global":
            self.global_types.append(parsed_type)
        else:
            parts = ns_name.split('.')
            top = parts[0]
            if top not in self.namespaces:
                self.namespaces[top] = Namespace(top, full_name=top)
            current_ns = self.namespaces[top]
            for part in parts[1:]:
                if part not in current_ns.sub_namespaces:
                    full = current_ns.full_name + '.' + part
                    current_ns.sub_namespaces[part] = Namespace(part, full_name=full)
                current_ns = current_ns.sub_namespaces[part]
            current_ns.add_type(parsed_type)

    def __repr__(self):
        ns_count = len(self.namespaces)
        gt_count = len(self.global_types)
        return f"<Project: {ns_count} top-level namespaces, {gt_count} global types>"

    def describe(self, indent: int = 0) -> str:
        pad = "  " * indent
        lines = [f"{pad}Project:"]
        lines.append(f"{pad}  Global Types:")
        if self.global_types:
            for typ in self.global_types:
                lines.append(typ.describe(indent + 2))
        else:
            lines.append(f"{pad}    None")
        lines.append(f"{pad}  Namespaces:")
        if self.namespaces:
            for ns in self.namespaces.values():
                lines.append(ns.describe(indent + 2))
        else:
            lines.append(f"{pad}    None")
        return "\n".join(lines)


# Conversion helper functions

def extract_generics(type_name: str) -> (str, Optional[str]):
    match = re.match(r'(\w+)\s*<\s*([^>]+)\s*>', type_name)
    if match:
        return match.group(1), match.group(2)
    return type_name, None

def convert_method(raw_method: Dict[str, Any]) -> Method:
    name = raw_method.get("name", "")
    static = raw_method.get("static", False)
    return_type = raw_method.get("return_type", None)
    parameters = []
    raw_params = raw_method.get("params", "")
    if raw_params:
        parameters = [
            Parameter(*param.split(" ", 1))  # naive split: "Type name"
            for param in raw_params.split(',')
            if param.strip()
        ]
    return Method(name=name, parameters=parameters, return_type=return_type, static=static)

def convert_property(raw_property: Dict[str, Any]) -> Property:
    name = raw_property.get("name", "")
    property_type = raw_property.get("type", "")
    body = raw_property.get("body", "")
    static = raw_property.get("static", False)
    annotations = raw_property.get("annotations", [])
    return Property(name=name, property_type=property_type, body=body, static=static, annotations=annotations)

def convert_parsed_type(raw: Dict[str, Any]) -> ParsedType:
    kind = raw.get("kind", "")
    full_name = raw.get("name", "")
    base_name, generics = extract_generics(full_name)
    bases = raw.get("bases", [])
    namespace = raw.get("namespace", "Global")
    methods = [convert_method(m) for m in raw.get("methods", [])]
    members = raw.get("members", [])
    properties = [convert_property(p) for p in raw.get("properties", [])]
    annotations = []  # Extend to capture annotations if available.
    return ParsedType(
        kind=kind,
        name=base_name,
        bases=bases,
        methods=methods,
        members=members,
        properties=properties,
        annotations=annotations,
        generics=generics,
        namespace=namespace
    )
