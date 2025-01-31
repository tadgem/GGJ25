shader_type spatial;

uniform sampler2D colour_texture;
uniform sampler2D noise_texture;
uniform float smoothness : hint_range(0.0, 1.0, 0.01);
uniform float time_scale : hint_range(0.0, 1.0, 0.01);
varying vec3 world_position;

void vertex() {
	// Called for every vertex the material is visible on.
	world_position = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	float height = texture(noise_texture, VERTEX.xz * smoothness+ vec2(TIME * time_scale)).r;
	VERTEX += NORMAL * height;
}

float fresnel(vec3 normal, mat4 camera_matrix)
{
	vec3 view_direction_world = (camera_matrix * vec4(0.0, 0.0, 1.0, 0.0)).xyz;
	vec3 normal_world = (camera_matrix * vec4(normal, 0.0)).xyz;
	
	float fresnel = dot(view_direction_world, normal_world);
	fresnel = clamp(abs(fresnel), 0.0, 1.0);
	return 1.0 - fresnel;
}

void fragment() {
	float fresnel = fresnel(NORMAL, INV_VIEW_MATRIX);
	ALBEDO = texture(colour_texture, world_position.xz * vec2(TIME * time_scale)).xyz;
	ALPHA = fresnel;
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
