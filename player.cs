using Godot;
using System;

public class player : KinematicBody2D
{
    Vector2 UP = new Vector2(0, -1);
    const int GRAVITY = 20;
    const int MAXFALLSPEED = 200;
    const int MAXSPEED = 100;
    const int JUMPFORCE = 300;

    const int ACCEL = 10;
    Vector2 vZero = new Vector2();

    bool facing_right = true;

    Vector2 motion = new Vector2();

    Sprite currentSprite;
    //AnimationPlayer animPlayer;
    AnimationNodeStateMachinePlayback animationState;
    AnimationTree animationTree;
        // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currentSprite = GetNode<Sprite>("Sprite");
        //animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationState = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
    }

    public override void _PhysicsProcess(float delta)
    {
        var input_vector  = Vector2.Zero;
        input_vector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        input_vector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_jump");
        input_vector = input_vector.Normalized();

        motion.y += GRAVITY;

        if(motion.y > MAXFALLSPEED) {
            motion.y = MAXFALLSPEED;
        }

        if (facing_right) {
            currentSprite.FlipH = false;
        } else {
            currentSprite.FlipH = true;
        }

         motion.x = motion.Clamped(MAXSPEED).x;

        if (Input.IsActionPressed("ui_left")) {
            motion.x -= ACCEL;
            facing_right = false;
            animationTree.Set("parameters/Run/blend_position",input_vector);
            animationState.Travel("Run");
        } else if (Input.IsActionPressed("ui_right")) {
            motion.x += ACCEL;
            facing_right = true;
            animationTree.Set("parameters/Run/blend_position",input_vector);
            animationState.Travel("Run");
        }else if(Input.IsActionPressed("attack")){
            animationTree.Set("parameters/Attack/blend_position",input_vector);
            animationState.Travel("Attack");
        }else {
            motion = motion.LinearInterpolate(Vector2.Zero, 0.2f);
            animationTree.Set("parameters/Idle/blend_position",input_vector);
            animationState.Travel("Idle");
        }

        if (IsOnFloor())
            // On ne regarde qu'un seul fois et non le maintient de la touche
            if (Input.IsActionJustPressed("ui_jump")) {
                motion.y = -JUMPFORCE;
                GD.Print($"motion.y = {motion.y}");
                Console.WriteLine($"motion.y = {motion.y}");
            }

        if (!IsOnFloor()) {
            if (motion.y < 0) {
                animationTree.Set("parameters/Jump/blend_position",input_vector);
                animationState.Travel("Jump");
            } else if (motion.y > 0) {
                animationTree.Set("parameters/Fall/blend_position",input_vector);
                animationState.Travel("Fall");
            }
        }

        motion = MoveAndSlide(motion, UP);
    }


    
}
