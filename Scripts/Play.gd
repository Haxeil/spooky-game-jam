extends TextureButton


var dont_change_anim = false


func _on_Play_mouse_entered():
	if not dont_change_anim:
		$AnimationPlayer.play("Hover")
	$Hover.play()


func _on_Play_mouse_exited():
	if not dont_change_anim:
		$AnimationPlayer.play("RESET")


func _on_Play_pressed():
	$AnimationPlayer.play("Click")
	$Click.play()
	dont_change_anim = true
func change_scene():
	get_tree().change_scene("res://scenes/Vid.tscn")


