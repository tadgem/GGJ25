shader_type spatial;

uniform float dither_threshold : hint_range(0.0, 0.1, 0.002);
uniform sampler2D colour_texture;

void fragment() {
	if(DEPTH < dither_threshold)
	{
		ALPHA = DEPTH;
	}
	ALBEDO = textureLod(colour_texture, UV, 0.0).xyz;
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
