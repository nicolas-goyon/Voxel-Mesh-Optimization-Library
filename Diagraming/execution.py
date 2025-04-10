import sys
import json
import importlib

def load_config(config_path):
    """Load a JSON external config file with default values."""
    default_config = {
        "exclude_files": [],
        "exclude_namespaces": [],  # Legacy: now moved inside output; kept here for backward compatibility if needed.
        "input_filetype": "Csharp",
        "output": {
            "mode": "console",         # "console" or "file"
            "file": "diagram.md",
            "diagram": "MermaidClassDiagram",
            "hide_implemented_interface_methods": True,
            "hide_implemented_interface_properties": True,
            "exclude_namespaces": []     # Now moved here under output.
        }
    }
    try:
        with open(config_path, "r", encoding="utf-8") as cf:
            user_config = json.load(cf)
            default_config.update(user_config)
    except Exception as e:
        print(f"Error loading config file: {e}")
        sys.exit(1)
    return default_config

def load_internal_config(internal_config_path="internal_config.json"):
    """Load the internal configuration that maps file types and diagram types to module paths."""
    try:
        with open(internal_config_path, "r", encoding="utf-8") as f:
            internal_config = json.load(f)
    except Exception as e:
        print(f"Error loading internal config file: {e}")
        sys.exit(1)
    return internal_config

def validate_config(config, internal_config):
    """
    Validate that the external config includes the expected input and output types,
    and that they exist in the internal mapping.
    """
    inp_type = config.get("input_filetype")
    out_type = config.get("output", {}).get("diagram")
    
    if inp_type not in internal_config.get("input_filetype_mapping", {}):
        print(f"Error: input_filetype '{inp_type}' is not supported by the internal mapping.")
        sys.exit(1)
    if out_type not in internal_config.get("output_diagram_mapping", {}):
        print(f"Error: output_diagram '{out_type}' is not supported by the internal mapping.")
        sys.exit(1)

def dynamic_import(module_path):
    """
    Dynamically import a module given its file path using dot notation.
    For example, "parsing/csharp_parsing.py" becomes "parsing.csharp_parsing".
    """
    module_dot = module_path.replace("/", ".").removesuffix(".py")
    try:
        return importlib.import_module(module_dot)
    except Exception as e:
        print(f"Error importing module '{module_dot}': {e}")
        sys.exit(1)

def main():
    if len(sys.argv) != 3:
        print(f"Usage: python {sys.argv[0]} <folder_path> <config_file>")
        sys.exit(1)
    
    folder_path = sys.argv[1]
    config_path = sys.argv[2]
    
    # Load external and internal configurations.
    config = load_config(config_path)
    internal_config = load_internal_config()
    
    # Validate configurations.
    validate_config(config, internal_config)
    
    # Dynamically load the parser module.
    input_type = config.get("input_filetype")
    parser_module_path = internal_config["input_filetype_mapping"][input_type]
    parser_module = dynamic_import(parser_module_path)
    
    # Dynamically load the diagram module.
    output_obj = config.get("output", {})
    output_type = output_obj.get("diagram")
    diagram_module_path = internal_config["output_diagram_mapping"][output_type]
    diagram_module = dynamic_import(diagram_module_path)
    
    # Use the parser module to parse the folder.
    classes = parser_module.parse_project(folder_path, exclude_files=config.get("exclude_files"))
    
    # print(classes.describe())
    # Dynamically instantiate the diagram generator using a factory function.
    try:
        # We assume that each diagram module exposes a function 'create_generator'
        # that takes (project, output_config) and returns an instance of a diagram generator.
        diagram_generator = diagram_module.create_generator(classes, output_obj)
    except AttributeError:
        print("Error: The diagram module does not expose the 'create_generator' factory function.")
        sys.exit(1)
    
    # Generate the diagram.
    diagram = diagram_generator.generate()
    
    # Output the diagram according to the output mode.
    if output_obj.get("mode", "console") == "file":
        output_file = output_obj.get("file", "diagram.md")
        try:
            with open(output_file, "w", encoding="utf-8") as out:
                out.write(diagram)
            print(f"Diagram written to {output_file}")
        except Exception as e:
            print(f"Error writing to file: {e}")
    else:
        print("Generated Diagram:")
        print(diagram)

if __name__ == "__main__":
    main()
