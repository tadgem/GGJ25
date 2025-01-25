shader_type spatial;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform sampler2D depth_texture : hint_depth_texture, repeat_disable, filter_nearest;

uniform float depth_distance 		: hint_range(0.0, 20.0, 0.03) = 5.0;
uniform float beers_law 			: hint_range(0.0, 20.0, 0.03) = 12.0;
uniform float water_transparency 	: hint_range(0.0, 1.0, 0.01) = 0.09;


void fragment() {
	float depth = texture(depth_texture, SCREEN_UV).r;
	// Called for every pixel the material is visible on.
	vec3 ndc = vec3(SCREEN_UV * 2.0 - 1.0, depth);
	vec4 world = INV_VIEW_MATRIX * INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
	float depth_texture_y = world.y / world.w;
	float vertex_y = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).y;
	
	float depth_blend = clamp((vertex_y - depth_texture_y) / depth_distance, 0.0, 1.0);
	depth_blend = exp(-depth_blend * beers_law);
	ALBEDO = vec3(depth_blend);
	ALPHA = 1.0 - water_transparency;
	EMISSION = vec3(0.0);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
