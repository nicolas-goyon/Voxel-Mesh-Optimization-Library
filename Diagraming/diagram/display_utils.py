
import re

def extract_base_types(type_str: str) -> list:
    # Remove any surrounding brackets, spaces, or parentheses.
    type_str = type_str.strip()

    # This regex finds words that start with a letter and may contain letters, numbers, or underscores.
    tokens = re.findall(r'\b[A-Za-z_]\w*\b', type_str)
    
    # Optionally filter out known generic types like List, Dictionary, etc.
    generic_types = {"List", "Dictionary", "Set", "Tuple", "Optional"}
    base_types = [token for token in tokens if token not in generic_types]
    return base_types


def should_exclude_package(package_name: str, excluded_packages: set) -> bool:
    """
    Checks if the provided package_name should be excluded.
    
    :param package_name: The full package name to check.
    :param excluded_packages: A set of package names to exclude.
    :return: True if the package_name matches or is a subpackage of any
             package in excluded_packages, otherwise False.
    """
    for excluded in excluded_packages:
        # Check if package is exactly the same as the excluded package
        # or if it is a subpackage (starts with the excluded package name and then a dot)
        if package_name == excluded or package_name.startswith(f"{excluded}."):
            return True
    return False