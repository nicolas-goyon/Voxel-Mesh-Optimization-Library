# plantuml_display.py

from .display_utils import extract_base_types, should_exclude_package


class PlantUMLDiagram:
    def __init__(self, project, output_config):
        self.project = project
        self.output_config = output_config or {}
        self.hide_methods = self.output_config.get("hide_implemented_interface_methods", True)
        self.hide_properties = self.output_config.get("hide_implemented_interface_properties", True)
        self.exclude_namespaces = set(self.output_config.get("exclude_namespaces", []))
        # New flag to enable dependency links
        self.show_dependencies = self.output_config.get("show_dependencies", False)

    def generate(self):
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

        # Render inheritance relationships.
        all_types = list(self.project.global_types)
        for ns_obj in self.project.namespaces.values():
            if ns_obj.full_name in self.exclude_namespaces:
                continue  # Skip entire namespaces
            all_types.extend(self.get_all_types(ns_obj))
        for ptype in all_types:
            if should_exclude_package(ptype.namespace, self.exclude_namespaces):
                continue
            for base in ptype.bases:
                diagram_lines.append(f"{ptype.name} --|> {base}")

        # Render dependency links if enabled.
        if self.show_dependencies:
            dependency_links = self.build_dependency_links(all_types)
            diagram_lines.extend(dependency_links)

        diagram_lines.append("@enduml")
        return "\n".join(diagram_lines)

    def build_dependency_links(self, all_types):
        """
        Build dependency links between types.
        A dependency is added if a type (class/interface) uses another type
        in its properties, method parameters, or method return types.
        For classes, if the dependency is already defined by one of its parent interfaces
        (and the corresponding hide flag is set), then the dependency arrow will be omitted.
        """
        # Create a lookup of types by name.
        type_lookup = {ptype.name: ptype for ptype in all_types}
        dependency_links = set()  # Using a set to avoid duplicates.

        for ptype in all_types:
            current_name = ptype.name
            if should_exclude_package(ptype.namespace, self.exclude_namespaces):
                continue
            # For classes, collect dependency types inherited from parent interfaces.
            inherited_dependency_types = set()
            if ptype.kind == "class":
                for base_name in ptype.bases:
                    if base_name in type_lookup:
                        parent = type_lookup[base_name]
                        if parent.kind == "interface":
                            # From parent's properties.
                            for prop in parent.properties:
                                inherited_dependency_types.add(prop.property_type.strip())
                            # From parent's methods.
                            for method in parent.methods:
                                for param in method.parameters:
                                    inherited_dependency_types.add(param.param_type.strip())
                                if method.return_type:
                                    inherited_dependency_types.add(method.return_type.strip())

            # Process property dependencies.
            for prop in ptype.properties:
                base_types = extract_base_types(prop.property_type)
                for base in base_types:
                    if base in type_lookup and base != current_name:
                        # If this class inherits an interface already using this type,
                        # and the config to hide interface properties is enabled, skip.
                        if (
                            ptype.kind == "class" and 
                            self.hide_properties and 
                            base in inherited_dependency_types
                        ):
                            continue
                        dependency_links.add(f"{current_name} ..> {base}")

            # Process method dependencies.
            for method in ptype.methods:
                # Check each parameter.
                for param in method.parameters:
                    base_types = extract_base_types(param.param_type)
                    for base in base_types:
                        if base in type_lookup and base != current_name:
                            # Skip if the parent interface already defines the dependency
                            # and the flag to hide methods is enabled.
                            if (
                                ptype.kind == "class" and 
                                self.hide_methods and 
                                base in inherited_dependency_types
                            ):
                                continue
                            dependency_links.add(f"{current_name} ..> {base}")
                # Check method return type.
                if method.return_type:
                    base_types = extract_base_types(method.return_type)
                    for base in base_types:
                        if base in type_lookup and base != current_name:
                            if (
                                ptype.kind == "class" and 
                                self.hide_methods and 
                                base in inherited_dependency_types
                            ):
                                continue
                            dependency_links.add(f"{current_name} ..> {base}")

        # Return the dependency links as a sorted list for consistency.
        return sorted(dependency_links)



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