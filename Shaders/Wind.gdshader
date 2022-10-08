shader_type canvas_item;
uniform float amount;
uniform sampler2D noise_pattern;
uniform float strength;
uniform float ycutoff;
uniform float speed = 1.0;
uniform float cell_size = 0.0933;


float rand(float x) {
	return fract(sin(x)*10000.0);
}

float noise(float offset) {
	return 2.0 * texture(noise_pattern, vec2(mod(TIME * speed + offset, 10.0)/10.0,
											 0)).r - 1.0;
}

void vertex() {
	//VERTEX += vec2((cos(TIME*speed)*strength + noise()) * (1.0 - min(1.0, UV.y / 0.5)), 0);
	vec2 CV = UV / cell_size;
	VERTEX += vec2(noise(mod(VERTEX.x, 32.0) / 32.0) *strength * (1.0 - min(1.0, CV.y / 0.5)), 0);
	//VERTEX += vec2((cos(TIME)*strength + noise()*0.0) * 0.0, 0);
}

void fragment() {
	COLOR = texture(TEXTURE, UV);
	vec4 newColor = COLOR * (1.0 - amount) + vec4(1,1,1, COLOR.a) * amount;
	newColor.a = COLOR.a;
	COLOR = newColor;
}