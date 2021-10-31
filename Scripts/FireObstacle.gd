extends Area2D


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
export(float) var damage = 50
export(Vector2) var damage_velocity = Vector2(0, 800);

# Called when the node enters the scene tree for the first time.



# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass


func _on_FireObstacle_body_entered(body):
	if $DelayHurt.is_stopped():
		body.ApplyDamage(damage, damage_velocity, 1);
	$DelayHurt.start()

func _on_VisibilityNotifier2D_screen_entered():
	$Anim.play("default")
	self.show()
	


func _on_VisibilityNotifier2D_screen_exited():
	$Anim.stop()
	self.hide()
