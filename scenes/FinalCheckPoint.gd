extends Area2D




func _on_CheckPoint_body_entered(body):
	if body is KinematicBody2D:
		$AnimatedSprite.play("A");
		body.StopBloodClot();
	pass # Replace with function body.



func _on_AnimatedSprite_animation_finished():
	get_parent().get_node("EndFill").show()
	get_parent().get_node("EndFill/AnimatedSprite").play("End");
	print_debug("Done")
