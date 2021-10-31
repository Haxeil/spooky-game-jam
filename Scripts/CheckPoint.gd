extends Area2D




func _on_CheckPoint_body_entered(body):
	if body is KinematicBody2D:
		$AnimatedSprite.play("A");
		body.StopBloodClot();
	pass # Replace with function body.
