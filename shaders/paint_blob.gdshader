shader_type spatial;
render_mode unshaded;

uniform vec4 paint_tint : source_color = vec4(1);
uniform sampler2D paint_tex : source_color, hint_default_white, repeat_enable;
uniform vec2 paint_tex_scale = vec2(1);

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec3 tex_sample = texture(paint_tex, UV * paint_tex_scale).rgb;
	vec3 tint = paint_tint.rgb;
	float fresnel = mix(pow(dot(NORMAL, VIEW), 1.0), 1.0, 0.6);
	float highlight = clamp(0, 1, pow(max(dot(NORMAL + vec3(0, -5, -0.2), VIEW), 0.0), 10));
	highlight *= 0.08;
	ALBEDO = COLOR.rgb * tex_sample * tint * fresnel + highlight;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
