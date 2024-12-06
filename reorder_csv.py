import pandas as pd
import os
import sys

def reorder_csv(input_file, output_file, desired_columns):
    try:
        # Normalize the header in the CSV file
        with open(input_file, 'r', encoding='utf-8') as f:
            lines = f.readlines()

        # Replace spaces with underscores and strip headers
        lines[0] = ";".join([header.strip().replace(" ", "_") for header in lines[0].split(";")])

        with open(input_file, 'w', encoding='utf-8') as f:
            f.writelines(lines)

        # Read the processed CSV file
        print(f"Reading input file: {input_file}")
        df = pd.read_csv(input_file, delimiter=';')

        # Reorder and select desired columns
        df = df[desired_columns]

        #Write output file
        df.to_csv(output_file, index=False, sep=';')
        print(f"Reordered CSV written to: {output_file}")

    except FileNotFoundError as e:
        print(f"Error: Input file not found: {e}")
    except KeyError as e:
        print(f"Error: Missing column(s) in the CSV: {e}")
    except Exception as e:
        print(f"An unexpected error occurred: {e}")

if __name__ == "__main__":
    # Example usage: Pass input_file, output_file, and desired_columns from C#
    input_file = sys.argv[1]
    output_file = sys.argv[2]
    desired_columns = sys.argv[3:]  # These should already be normalized by C#

    reorder_csv(input_file, output_file, desired_columns)
