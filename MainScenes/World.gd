extends Node2D


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _process(delta):
	if Input.is_action_pressed("ui_cancel"):
		get_tree().change_scene("res://scenes/MainUI.tscn")


func _on_Area2D_body_entered(body):
	if not $Sountracks/InsideTrack.playing:
		$Sountracks/InsideTrack.play()
		$Inside.queue_free()


func _on_End_timeout():
	get_tree().change_scene("res://scenes/EndUI.tscn")


func _on_FinalCheckPoint_body_entered(body):
	pass # Replace with function body.


func _on_AnimatedSprite_animation_finished():
	print_debug("so")
	$EndFill.show()
	$EndFill/AnimatedSprite.play()
