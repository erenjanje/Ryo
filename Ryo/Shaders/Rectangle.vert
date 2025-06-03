#version 460

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Rectangle;
layout(location = 2) in vec4 Texture;

out vec2 TextureCoordinates;

void main() {
    vec2 rectanglePosition = Rectangle.xy;
    vec2 rectangleSize = Rectangle.zw;
    vec2 texturePosition = Texture.xy;
    vec2 textureSize = Texture.zw;
    vec2 position = (vec2(Position.x, Position.y) * rectangleSize + rectanglePosition) * 2.0f - 1.0f;
    gl_Position = vec4(position.x, -position.y, 0.0f, 1.0f);
    vec2 textureCoordinates = vec2(Position * textureSize + texturePosition);
    TextureCoordinates = textureCoordinates;
}