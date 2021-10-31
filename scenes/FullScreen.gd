extends TextureButton


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
var dont_change_anim = false

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass


func _on_FullScreen_mouse_entered():
	if not dont_change_anim:
		$AnimationPlayer.play("Hover")
	$Hover.play()

func _on_FullScreen_mouse_exited():
	if not dont_change_anim:
		$AnimationPlayer.play("RESET")
func reset():
	$AnimationPlayer.play("RESET")

func _on_FullScreen_pressed():
	$AnimationPlayer.play("Click")
	dont_change_anim = true
	$Click.play()

func full_screen():
	if OS.window_fullscreen == false:
		OS.window_fullscreen = true
	else:
		OS.window_fullscreen = false;


