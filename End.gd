extends Control



func _process(delta):
	if Input.is_action_pressed("ui_cancel"):
		get_tree().change_scene("res://scenes/MainUI.tscn")


func _on_Timer_timeout():
	$Dead.play()
