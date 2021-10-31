extends TextureButton

var dont_change_anim = false

func _on_Exit_mouse_entered():
	if not dont_change_anim:
		$AnimationPlayer.play("Hover")
	$Hover.play()

func _on_Exit_mouse_exited():
	if not dont_change_anim:
		$AnimationPlayer.play("RESET")


func _on_Exit_pressed():
	$AnimationPlayer.play("Click")
	dont_change_anim = true
	$Click.play()


func exit():
	get_tree().quit()



