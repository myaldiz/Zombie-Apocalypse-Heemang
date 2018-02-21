using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackSound : StateMachineBehaviour {

	private AudioSource zombie_attack_audio;
	public AudioClip zombie_attack_sound;


	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		this.zombie_attack_audio = animator.GetComponent<AudioSource> ();
		this.zombie_attack_audio.clip = zombie_attack_sound;
		this.zombie_attack_audio.volume = 0.06f;
		this.zombie_attack_audio.loop = false;
		this.zombie_attack_audio.PlayDelayed (1);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
