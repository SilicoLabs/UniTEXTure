# UniTEXTure

An editor window in Unity that interacts with the TEXTure program to generate textures for 3D objects

## Prerequisites

- [TEXTure](https://github.com/TEXTurePaper/TEXTurePaper): A program for generating textures.
- [ProtoBuf](https://protobuf.dev/): Protocol Buffers by Google.

## Getting Started

Step-by-step instructions on setting up and running the program.ex

### Step 1: Clone the repository
Navigate to the directory where you want the file saved and do:

```bash
git clone https://github.com/SilicoLabs/UniTEXTure.git
```

### Step 2: Clone TEXTure
Navigate inside the UniTEXTure repository and if TEXTure is not already there, do:
```bash
git clone https://github.com/TEXTurePaper/TEXTurePaper.git
```

**Note:** make sure all TEXTure requirements are also installed

## Running the Program

### Step 3: Run the python server
Navigate to your AWS instance and run `python py_communication.py`. You should see output information that the server is running. If that doesn't appear, check your python and flask version and packages to see if there are any updates.

Next, run `ngrok http YOUR_PORT_NUM` to expose a secure tunnel to the internet so Unity can communicate with it.

**Note:** we are currently using Python 3.11.5.

### Step 4: Unity
Import UniTEXTure Project into Unity and run it. Once the Unity editor is open, click the `UniTEXTure` dropdown (should be between `Component` and `Window`) and click `create`.

### Step 5: Fill out the form
Once you have the texture generation window open, fill out the following:

- **Prompt:** describe the texture you want generated.
- **Experiment Name:** what the yaml file will be named (no effect on the texture generation).
- **Seed:** different numbers will output slightly different textures.
- **Input Texture *(optional)*:** a pre-existing texture that TEXTure can build off of. If not selected, it will generate a texture off of just the prompt.
- **Object File:** This is the 3D model that is to be textured.

Once you have filled out the form, click `Generate New Texture`. This will send the created `.proto` object to the python server. It is received and passed to the TEXTure program for generation. This may take a while (~15 minutes), but once the texture is generated, it is sent back to the requester and saved.
