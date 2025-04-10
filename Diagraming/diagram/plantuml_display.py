# plantuml_display.py

class PlantUMLDiagram:
    def __init__(self, project, output_config):
        """
        Initialize the PlantUMLDiagram generator.

        :param project: The project instance containing global types, namespaces, etc.
        :param output_config: Dictionary containing output settings, e.g.:
             {
                 "mode": "file",               // "console" or "file"
                 "file": "diagram.md",
                 "diagram": "PlantUML",
                 "hide_implemented_interface_methods": true,
                 "hide_implemented_interface_properties": true,
                 "exclude_namespaces": [ ... ]  // Namespaces to be excluded from the diagram.
             }
        """
        self.project = project
        self.output_config = output_config or {}
        self.hide_methods = self.output_config.get("hide_implemented_interface_methods", True)
        self.hide_properties = self.output_config.get("hide_implemented_interface_properties", True)
        self.exclude_namespaces = set(self.output_config.get("exclude_namespaces", []))
    
    def generate(self):
        """
        Generate and return a PlantUML class diagram as a string.
        
        The method makes use of the excluded namespaces specified in the output configuration.
        :return: A string containing the PlantUML diagram.
        """
        diagram_lines = ["@startuml"]

        # Build dictionaries for interface methods and properties.
        interface_methods_dict = self.build_interface_methods_dict()
        interface_properties_dict = self.build_interface_properties_dict()

        # Render global types.
        for ptype in self.project.global_types:
            diagram_lines.extend(self.render_type(
                ptype,
                indent="",
                interface_methods=interface_methods_dict,
                interface_properties=interface_properties_dict
            ))

        # Render each namespace as a package.
        for ns_name in sorted(self.project.namespaces):
            ns_obj = self.project.namespaces[ns_name]
            if ns_obj.full_name in self.exclude_namespaces:
                continue
            diagram_lines.append(f'package "{ns_obj.full_name}" {{')
            diagram_lines.extend(self.render_namespace(
                ns_obj,
                indent="  ",
                interface_methods=interface_methods_dict,
                interface_properties=interface_properties_dict
            ))
            diagram_lines.append("}")

        diagram_lines.append("")  # Blank line before relationships.

        # Gather all types (global and nested) to render inheritance relationships.
        all_types = list(self.project.global_types)
        for ns_obj in self.project.namespaces.values():
            all_types.extend(self.get_all_types(ns_obj))

        # Render inheritance relationships.
        for ptype in all_types:
            for base in ptype.bases:
                diagram_lines.append(f"{ptype.name} --|> {base}")

        diagram_lines.append("@enduml")
        return "\n".join(diagram_lines)

    def build_interface_methods_dict(self):
        """
        Build a dictionary mapping interface names to a set of method signatures.
        """
        interface_methods = {}
        all_types = list(self.project.global_types)
        for ns_obj in self.flatten_namespaces(self.project.namespaces):
            all_types.extend(ns_obj.types)
        for ptype in all_types:
            if ptype.kind == "interface":
                sig_set = set()
                for method in ptype.methods:
                    sig_set.add(self.get_method_signature(method))
                interface_methods[ptype.name] = sig_set
        return interface_methods

    def build_interface_properties_dict(self):
        """
        Build a dictionary mapping interface names to a set of property signatures.
        """
        interface_properties = {}
        all_types = list(self.project.global_types)
        for ns_obj in self.flatten_namespaces(self.project.namespaces):
            all_types.extend(ns_obj.types)
        for ptype in all_types:
            if ptype.kind == "interface":
                sig_set = set()
                for prop in ptype.properties:
                    sig_set.add(self.get_property_signature(prop))
                interface_properties[ptype.name] = sig_set
        return interface_properties

    @staticmethod
    def get_method_signature(method):
        """
        Build a method signature in the form: methodName(paramType1,paramType2,...)
        """
        param_types = ",".join(p.param_type.strip() for p in method.parameters)
        return f"{method.name}({param_types})"

    @staticmethod
    def get_property_signature(prop):
        """
        Build a property signature in the form: propertyName:propertyType
        """
        return f"{prop.name.strip()}:{prop.property_type.strip()}"

    @staticmethod
    def flatten_namespaces(ns_dict):
        """
        Recursively flatten a dictionary of namespace objects into a list.
        """
        result = []
        for ns_obj in ns_dict.values():
            result.append(ns_obj)
            if ns_obj.sub_namespaces:
                result.extend(PlantUMLDiagram.flatten_namespaces(ns_obj.sub_namespaces))
        return result

    def render_namespace(self, ns_obj, indent="", interface_methods=None, interface_properties=None):
        """
        Render a namespace block in PlantUML syntax.
        """
        lines = []
        # Render types in the current namespace.
        for ptype in ns_obj.types:
            lines.extend(self.render_type(
                ptype,
                indent=indent,
                interface_methods=interface_methods,
                interface_properties=interface_properties
            ))
        
        # Render sub-namespaces recursively.
        for sub in sorted(ns_obj.sub_namespaces.values(), key=lambda s: s.name):
            if sub.full_name in self.exclude_namespaces:
                continue
            lines.append(f'{indent}package "{sub.full_name}" {{')
            lines.extend(self.render_namespace(
                sub,
                indent=indent + "  ",
                interface_methods=interface_methods,
                interface_properties=interface_properties
            ))
            lines.append(f'{indent}}}')
        return lines

    def render_type(self, ptype, indent="", interface_methods=None, interface_properties=None):
        """
        Render a single type (class, interface, or enum) in PlantUML syntax.
        Uses the configured flags to hide interface-implemented members.
        """
        lines = []
        kind = ptype.kind
        name = ptype.name

        if kind in ("class", "interface"):
            type_keyword = "class" if kind == "class" else "interface"
            lines.append(f'{indent}{type_keyword} {name} {{')
            
            implemented_prop_signatures = set()
            implemented_method_signatures = set()
            if kind == "class":
                if self.hide_properties:
                    for base in ptype.bases:
                        if base in interface_properties:
                            implemented_prop_signatures |= interface_properties[base]
                if self.hide_methods:
                    for base in ptype.bases:
                        if base in interface_methods:
                            implemented_method_signatures |= interface_methods[base]
            
            # Render properties.
            for prop in ptype.properties:
                prop_sig = self.get_property_signature(prop)
                if kind == "class" and self.hide_properties and prop_sig in implemented_prop_signatures:
                    continue
                prop_name = prop.name
                if getattr(prop, "static", False):
                    prop_name = "«static» " + prop_name
                lines.append(f'{indent}  + {prop_name}: {prop.property_type}')

            # Render methods.
            for method in ptype.methods:
                sig = self.get_method_signature(method)
                if kind == "class" and self.hide_methods and sig in implemented_method_signatures:
                    continue
                method_name = method.name
                if getattr(method, "static", False):
                    method_name = "«static» " + method_name
                params = ", ".join(f"{p.param_type} {p.name}" for p in method.parameters)
                ret = f" : {method.return_type}" if method.return_type else ""
                lines.append(f'{indent}  + {method_name}({params}){ret}')
            lines.append(f'{indent}}}')
        elif kind == "enum":
            lines.append(f'{indent}enum {name} {{')
            for member in ptype.members:
                lines.append(f'{indent}  {member}')
            lines.append(f'{indent}}}')
        else:
            lines.append(f'{indent}class {name} {{}}')
        
        return lines

    def get_all_types(self, ns_obj):
        """
        Recursively gather all types defined within a namespace (and its sub-namespaces).
        """
        types = list(ns_obj.types)
        for sub in ns_obj.sub_namespaces.values():
            types.extend(self.get_all_types(sub))
        return types

def create_generator(project, output_config):
    return PlantUMLDiagram(project, output_config)