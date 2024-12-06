import pandas as pd
import os
import sys

def reorder_csv(input_file, output_file, desired_columns):
    try:
        # Check if output file exists, and notify if it will be created
        if not os.path.exists(output_file):
            print(f"Output file does not exist and will be created: {output_file}")

        # Read the input CSV file
        print(f"Reading input file: {input_file}")
        df = pd.read_csv(input_file, delimiter=';')

        # Replace spaces in column names with underscores
        df.columns = df.columns.str.replace(' ', '_')

        # Convert desired columns to use underscores as well
        desired_columns = [col.replace(' ', '_') for col in desired_columns]

        # Reorder and select desired columns
        df = df[desired_columns]

        # Write the reordered columns to the output file
        df.to_csv(output_file, index=False, sep=';')
        print(f"Reordered CSV written to: {output_file}")
    except FileNotFoundError as e:
        print(f"Error: Input file not found: {e}")
    except KeyError as e:
        print(f"Error: Missing column(s) in the CSV: {e}")
    except Exception as e:
        print(f"An unexpected error occurred: {e}")

if __name__ == "__main__":
    # Accept arguments from C#
    input_file = sys.argv[1]
    output_file = sys.argv[2]
    desired_columns = sys.argv[3:]

    reorder_csv(input_file, output_file, desired_columns)

# Example usage
#desired_columns = ["ID Utente", "Login", "Nome", "Cognome", "Email", "Codice Fiscale", "Cellulare", "Indirizzo", "Città", "Codice Postale", "Paese", "Provincia", "Regione"]
