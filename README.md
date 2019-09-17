# StoryProgramming

## Shaders
<b>Shader graph: Dissolve effect</b> – effect of appearing/disappearing.

<b>Shader graph: Scan effect</b> – effect that uses depth intersection to draw mesh parts only where mesh collides with the scene geometry.

<b>Shade graph: Force shield with hits</b> – force shield effect that uses Custom function Node to display hits and support dissolving.

<b>Shader graph: Rigid body animation using vertex animation textures</b> – generator that can record animation, create a combined mesh, and generate vertex animation textures. Vertex animation textures are then used by special shader to play the recorded animation on a single mesh transforming all parts of the mesh on GPU.

## Mesh manipulation
<b>Double-sided mesh generator</b> – generator that can generate a double-sided mesh from any given mesh. Double-sided meshes can be useful for some effects like force shields
