shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform bool effect_active = true;
uniform float tolerance = 0.2;
uniform vec3 target_color : source_color = vec3(0.0, 1.0, 0.0); // The color to keep

void fragment() {
    vec4 color = texture(screen_texture, SCREEN_UV);
    
    if (effect_active) {
        // Calculate how close the pixel is to our target color
        float dist = distance(color.rgb, target_color);
        
        if (dist > tolerance) {
            // Turn to greyscale if it's NOT the target color
            float luminance = dot(color.rgb, vec3(0.299, 0.587, 0.114));
            COLOR = vec4(vec3(luminance), color.a);
        } else {
            // Keep original color
            COLOR = color;
        }
    } else {
        COLOR = color;
    }
}