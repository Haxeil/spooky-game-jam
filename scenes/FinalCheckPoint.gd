extends "res://Scripts/CheckPoint.gd"






func _on_AnimatedSprite_animation_finished():
	get_parent().get_node("EndFill").show()
	get_parent().get_node("EndFill/AnimatedSprite").play("End");
	print_debug("Done")
