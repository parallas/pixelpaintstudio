shader_type canvas_item;

#include "shading.gdshaderinc"

uniform sampler2D paper_tex : source_color, hint_default_white, repeat_enable;
uniform float paper_intensity : hint_range(0.0, 1.0) = 1.0;
uniform float paper_power = 1.0;
uniform float paper_scale = 1.0;

instance uniform float color_blend : hint_range(0.0, 1.0) = 1.0;

varying vec4 vertex_color;
void vertex() {
	// Called for every vertex the material is visible on.
	vertex_color = COLOR;
}

void fragment() {
	vec4 paper_sample = texture(paper_tex, SCREEN_UV * vec2(paper_scale));
	paper_sample = pow(paper_sample, vec4(paper_power));
	paper_sample.a = mix(0.0, paper_sample.a, paper_intensity);
	paper_sample = mix(vec4(1.0), vec4(paper_sample.rgb, 1.0), paper_sample.a);
	
	vec3 color = COLOR.rgb;
	vec4 texture_sample = texture(TEXTURE, UV);
	
	vec3 texture_tint = texture_sample.rgb;
	texture_tint = rgb2hsv(texture_tint);
	texture_tint.g *= color_blend;
	texture_tint.b *= mix(0.8, 1.2, color_blend);
	texture_tint = hsv2rgb(texture_tint);
	texture_sample.rgb = texture_tint;
	
	COLOR = texture_sample * vertex_color * paper_sample;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
