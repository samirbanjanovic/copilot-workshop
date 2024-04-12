import os
import matplotlib.pyplot as plt
import matplotlib.backends.backend_pdf as pdf
from collections import defaultdict

# Dictionary to correlate file extension and language
languages = {
    '.py': 'Python',
    '.js': 'JavaScript',
    '.java': 'Java',
    '.cs': 'C#',
    '.cpp': 'C++',
    '.go': 'Go',
    '.ts': 'TypeScript',
    '.php': 'PHP',
    '.swift': 'Swift',
    '.kt': 'Kotlin',
    '.rb': 'Ruby',
    '.rs': 'Rust'
}

def get_language_summary(directory_info):
    # summarize data by language
    language_summary = defaultdict(int)
    for dirpath, files_info in directory_info.items():
        for file_info in files_info:
            language_summary[file_info['type']] += 1

    return language_summary

# function to generate pdf using language_summary data
def generate_pdf(language_summary, language_summary_chart):
    # generate pdf
    pdf_filename = 'report.pdf'
    pdf = pdf.PdfPages(pdf_filename)

    # generate pie chart
    labels = language_summary.keys()
    sizes = language_summary.values()


def generate_language_summary_report(language_summary_dict):
    
    # print summary
    print('Language Summary')
    print('----------------')
    for language, count in language_summary_dict.items():
        print(f"{language}: {count}")

    
    # generate pie chart
    language_labels = language_summary_dict.keys()
    language_counts = language_summary_dict.values()

    fig, (ax1, ax2) = plt.subplots(2, 1)  # 2 rows, 1 column
    ax1.pie(language_counts, labels=language_labels, autopct='%1.1f%%', startangle=90)
    ax1.axis('equal')  # Equal aspect ratio ensures that pie is drawn as a circle.

    # generate summary text
    summary_text = '\n'.join(f"{language}: {count}" for language, count in language_summary_dict.items())

    # add summary text to the second subplot
    ax2.axis('off')  # hide the axes
    ax2.text(0.5, 0.5, summary_text, fontsize=12, ha='center', va='center', transform=ax2.transAxes)

    return fig
"""

    Walk the directory tree and collect information about the files

"""
def walk_tree(directory):
    # Initialize a dictionary to store the information
    info_dict = defaultdict(list)

    # Walk the directory tree
    for dirpath, dirnames, filenames in os.walk(directory):
        # Exclude 'node_modules' subdirectory
        if 'node_modules' in dirnames:
            dirnames.remove('node_modules')
        

        # For each file, collect the required information
        for filename in filenames:
            filepath = os.path.join(dirpath, filename)
            file_extension = os.path.splitext(filename)[1]
            
            if file_extension in languages:
                try:                
                    file_info = {
                        'type': languages.get(file_extension, 'other'),
                        'size': os.path.getsize(filepath),
                        'date_created': os.path.getctime(filepath)
                    }
                except FileNotFoundError:
                    print(f"File not found: {filepath}")
                    continue
                # Group the information by directory
                info_dict[dirpath].append(file_info)

    return info_dict

# prompt user for directory to walk
directory = input('Enter the directory to walk: ')
directory = os.path.expanduser(directory)
# Use the function
directory_info = walk_tree(directory)
# check if directory is valid and exists
# verify directory exists in file system
if not os.path.isdir(directory):
    print(f"{directory} is not a valid directory")
    exit()

language_summary = get_language_summary(directory_info)
chart = generate_language_summary_report(language_summary)

# save the chart to a file
chart_filename = 'language_summary.png'
chart.savefig(chart_filename)
