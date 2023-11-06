import argparse
import subprocess

def run_texture_generator_with_config(yaml_config_path):
    try:
        # Define the command to run the Texture Generator
        command = [
            "python", "..\\TEXTurePaper\\scripts\\run_texture.py",
            "--config_path", yaml_config_path
        ]

        # Run the Texture Generator as a subprocess
        result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)

        # Check the result and handle output
        if result.returncode == 0:
            # Texture Generator ran successfully
            output = result.stdout
            print("TEXTure Output:", output)
        else:
            # There was an error running Texture Generator
            error_message = result.stderr
            print("Error running TEXTure:", error_message)

    except subprocess.CalledProcessError as e:
        # An error occurred when running the process
        print("Error running Texture Generator:", e)

if __name__ == "__main__":
    # Parse command line arguments
    parser = argparse.ArgumentParser(description="PCP for Texture Generator")
    parser.add_argument("--config_file", required=True, help="Path to the YAML configuration file")
    args = parser.parse_args()

    # Call the function to run the Texture Generator with the YAML configuration file
    run_texture_generator_with_config(args.config_file)
