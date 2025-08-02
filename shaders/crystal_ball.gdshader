shader_type spatial;
render_mode unshaded;

uniform sampler2D paint_tex : source_color, hint_default_black, repeat_enable;

void vertex() {
	// Called for every vertex the material is visible on.
}

// Bread and buter: Generates the MatCap's UV's
vec2 generate_matcap_uv(vec3 _normal, vec3 _node_position_view) {
	vec3 cross_space = cross(normalize(_node_position_view), _normal); // cross products the normal with it's base position
	vec2 mat_space = vec2(cross_space.y * 1.0, cross_space.x); // converts to a Vec2 UV, having to switch x and y
	mat_space *= vec2(-0.5, -0.5); // modifies the mat space's size to fit into uv coords, & flips it
	mat_space += vec2(0.5, 0.5); // transforms the mat space into uv coords
	return mat_space; // export the final uv's
}

void fragment() {
	vec2 matcap_uv = (generate_matcap_uv(NORMAL, NODE_POSITION_VIEW)); // Matcap UV's
	
	// Called for every pixel the material is visible on.
	vec3 tex_sample = texture(paint_tex, matcap_uv).rgb;
	float fresnel = mix(pow(dot(NORMAL, VIEW), 1.0), 1.0, 0.6);
	float highlight = clamp(0, 1, pow(max(dot(NORMAL + vec3(0, -5, -0.2), VIEW), 0.0), 10));
	highlight *= 0.08;
	vec3 final_color = COLOR.rgb * tex_sample * fresnel + highlight;
	//final_color = mix(final_color, vec3(1, 1, 1), vec3(1.0 - stencil_tex_sample));
	ALBEDO = final_color;
}