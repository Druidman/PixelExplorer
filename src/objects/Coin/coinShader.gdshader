shader_type canvas_item;

uniform vec3 albedo : source_color;
global uniform vec4 layer_color; // This links to the project setting

void fragment() {
    ALBEDO = albedo * layer_color.rgb;
}
