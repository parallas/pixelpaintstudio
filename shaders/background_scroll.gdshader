shader_type canvas_item;

uniform vec2 scroll_speed = vec2(0.5, 0.5);
varying vec4 vertex_color;

vec2 rotated(vec2 a, float angle) {
  float s = sin(angle);
  float c = cos(angle);
  vec2 res = a;

  res.x = a.x * c - a.y * s;
  res.y = a.x * s + a.y * c;

  return res;
}

void vertex() {
	vertex_color = COLOR;
}

void fragment() {
	vec2 scroll_uv = rotated(UV, 0.05);
	scroll_uv += TIME * scroll_speed;
	COLOR = texture(TEXTURE, scroll_uv) * vertex_color;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
