shader_type canvas_item;
render_mode unshaded, blend_premul_alpha;
#include "shading.gdshaderinc"

uniform sampler2D stencil_tex : source_color, hint_default_white, repeat_enable, filter_nearest;
uniform vec2 stencil_size;
uniform vec2 texture_size; 

void fragment() {
	vec4 stencil_sample = texture(stencil_tex, UV * (texture_size / stencil_size));
	float stencil_alpha = stencil_sample.a * stencil_sample.r;
	if (stencil_alpha <= 0.5) discard;
	
	vec3 color = COLOR.rgb;
	
	COLOR = COLOR;
	
}
