shader_type spatial;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform sampler2D depth_texture : hint_depth_texture, repeat_disable, filter_nearest;
uniform sampler2D normal_texture : hint_normal;

uniform float depth_distance 		: hint_range(0.0, 20.0, 0.03) = 5.0;
uniform float beers_law 			: hint_range(0.0, 20.0, 0.03) = 12.0;
uniform float water_transparency 	: hint_range(0.0, 1.0, 0.01) = 0.09;
uniform float time_scale : hint_range(0.0, 1.0, 0.01) = 0.5;
uniform float noise_scale : hint_range(0.0, 100.0, 0.01) = 0.5;
uniform float roughness : hint_range(0.0, 1.0, 0.01) = 0.1;

uniform vec4 water_colour : source_color;
uniform vec3 water_secondary_colour : source_color;

varying vec3 world_pos;
varying float height;

void vertex()
{
	world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	height = texture(normal_texture, world_pos.xz / noise_scale + TIME * time_scale).r;
}

float fresnel(float amount, vec3 normal, vec3 view)
{
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0 )), amount);
}


void fragment() {

	vec3 screen = texture(screen_texture, SCREEN_UV).xyz;
	float depth = texture(depth_texture, SCREEN_UV).r;
	// Called for every pixel the material is visible on.
	vec3 ndc = vec3(SCREEN_UV * 2.0 - 1.0, depth);
	vec4 world = INV_VIEW_MATRIX * INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
	float depth_texture_y = world.y / world.w;
	float vertex_y = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).y;

	float depth_blend = clamp((vertex_y - depth_texture_y) / depth_distance, 0.0, 1.0);
	depth_blend = exp(-depth_blend * beers_law);

	vec3 depth_colour = mix(water_secondary_colour.xyz, water_colour.xyz, depth_blend);
	vec3 final_colour = mix(depth_colour, screen, water_transparency);
	float basic_fresnel = fresnel(3.0, NORMAL, VIEW);

	vec3 surface_normal = texture(normal_texture, world_pos.xz * TIME * time_scale).xyz;
	NORMAL_MAP = surface_normal;
	ALBEDO = mix(final_colour, screen, depth_blend * (1.0 - basic_fresnel));
	ROUGHNESS = roughness;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
