'''
This script crawls my local Git root directory and generates a summary of the
languages used, percentage breakdown for each language, total number of files per language
'''

import os
from collections import defaultdict

def analyze_file_extensions(path):
    # Define known language extensions
    known_extensions = {'.c', '.java', '.py', '.cpp', '.hpp', '.cc', '.hh', '.cxx', '.hxx', '.cs', '.vb', '.js', '.php', '.swift', '.sql'}

    # Initialize a dictionary to count file extensions
    extension_counts = defaultdict(int)

    # Walk through the directory
    for dirpath, dirnames, filenames in os.walk(path):
        # For each file, increment the count of its extension if it's known
        for filename in filenames:
            extension = os.path.splitext(filename)[1]
            if extension in known_extensions:
                extension_counts[extension] += 1

    # Compute statistics
    total_files = sum(extension_counts.values())
    unique_extensions = len(extension_counts)
    most_common_extension = max(extension_counts, key=extension_counts.get)
    most_common_extension_count = extension_counts[most_common_extension]

    # Return statistics
    return {
        'total_files': total_files,
        'unique_extensions': unique_extensions,
        'most_common_extension': most_common_extension,
        'most_common_extension_count': most_common_extension_count,
    }

# Usage
path = '/path/to/directory'
stats = analyze_file_extensions(path)
print(stats)
