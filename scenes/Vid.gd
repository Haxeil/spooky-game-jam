extends Control


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _process(delta):
	if Input.is_action_pressed("ui_cancel"):
		_on_Change_timeout()
		


func _on_Timer_timeout():
	$VideoPlayer.play();
	$Change.start()


func _on_VideoPlayer_finished():
	print_debug("AnimFinished")
	get_tree().change_scene("res://MainScenes/World.tscn")


func _on_Change_timeout():
	$VideoPlayer.stop()
	get_tree().change_scene("res://MainScenes/World.tscn")

