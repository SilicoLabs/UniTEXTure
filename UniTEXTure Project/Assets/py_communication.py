from flask import Flask, request, send_file
from UniTexture_pb2 import UniTexture  # Import the generated Protobuf class
import subprocess
import yaml
import os

app = Flask(__name__)

def create_yaml(exp_name, prompt, texture, obj_file, seed, folder_path):
    # Define the YAML content to be as expected from TEXTure
    yaml_content = {
        "log": {
            "exp_name": exp_name
        },
        "guide": {
            "text": prompt,
            "initial_texture": "../" + f"{texture}" if texture else None,
            "shape_path": "../" + f"{obj_file}"
        },
        "optim": {
            "seed": seed
        }
    }

    # Write the YAML to a file and save it in the experiment folder
    file_name = f"{exp_name}.yaml"
    file_path = os.path.join(folder_path, file_name)

    with open(file_path, "w") as yaml_file:
        yaml.dump(yaml_content, yaml_file, default_flow_style=False)

    print(f"YAML file created: {file_path}")

    return file_path

@app.route('/process_texture', methods=['POST'])
def process_texture():
    print("Signal received...")
    data = request.get_data()
    message = UniTexture()
    message.ParseFromString(data)

    exp_name = message.exp_name
    prompt = message.prompt
    seed = message.seed
    init_texture = message.init_texture
    obj_file = message.obj_file

    # Assuming 'experiments' folder already exists
    experiment_folder = os.path.join('experiments', exp_name)
    
    # Create the experiment folder if it doesn't exist
    os.makedirs(experiment_folder, exist_ok=True)

    # Convert the byte[] to the actual .png file and save it in the experiment folder
    texture_file_path = os.path.join(experiment_folder, f'{exp_name}_texture.png')
    with open(texture_file_path, "wb") as texture_file:
        texture_file.write(init_texture)
        print(f"Texture file created in {texture_file_path}")

    # Convert the byte[] to the actual .obj file and save it in the experiment folder
    obj_file_path = os.path.join(experiment_folder, f'{exp_name}_file.obj')
    with open(obj_file_path, "wb") as obj_file_created:
        obj_file_created.write(obj_file)
        print(f"Object file created in {obj_file_path}")

    print(f"ExpName={exp_name}, Prompt={prompt}, Seed={seed}")
    
    yaml_path = "../" + create_yaml(exp_name, prompt, texture_file_path, obj_file_path, seed, experiment_folder)
    
    # Change directory to TEXTure and run it with the YAML file
    os.chdir("/home/ec2-user/SageMaker/TEXTurePaper")
    
    print("Running TEXTure, please wait...")
    subprocess.run(["python3", "-m", "scripts.run_texture", f"--config_path={yaml_path}"])
    
    print("Texture has been generated, sending file back to host.")
    
    # Define the output file path (you may need to adjust this based on your actual file structure)
    output_file_path = os.path.join("TEXTurePaper", "experiments", exp_name, "results", "step_00010_rgb.png")
    
    # Return the generated texture file as an attachment
    return send_file(output_file_path, mimetype='image/png', as_attachment=True)

if __name__ == '__main__':
    # Run the Flask app on all available network interfaces and port 8080
    app.run(host='0.0.0.0', port=8080)
