using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Menu : MonoBehaviour {

    public MenuType menuType;
    private Animator _animator;
    private Action hideCallback;

	void Start () {
        GetReferences();
    }

    protected virtual void GetReferences()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual void Show() {
        if (_animator == null)
            _animator = GetComponent<Animator>();
        _animator.SetBool("IsOpen", true);
    }

    public virtual void Hide(Action callback) {
        if (_animator == null)
            _animator = GetComponent<Animator>();
        _animator.SetBool("IsOpen", false);
        hideCallback = callback;
    }

    /// <summary>
    /// Should be called from animation event when hide animation ends.
    /// </summary>
    public void HideAnimationEnded() {
        if (hideCallback != null)
        {
            hideCallback.Invoke();
            hideCallback = null;
        }
}
}
