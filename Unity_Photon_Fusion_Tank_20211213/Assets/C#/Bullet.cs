using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// �l�u���ʳt�סB�s���ɶ�
/// </summary>
public class Bullet : NetworkBehaviour
{
    #region ��

    #endregion

    #region ���

    [Header("���ʳt��"), Range(0, 100)]
    public float speed = 5f;
    [Header("�s���ɶ�"), Range(0, 10)]
    public float lifeTime = 5f;

    #endregion

    #region �ݩ�

    // Networked �s�u���ݩʸ��
    /// <summary>
    /// �s���p�ɾ�
    /// </summary>
    [Networked]
    private TickTimer life { get; set; }

    #endregion

    #region ��k

    /// <summary>
    /// ��l���
    /// </summary>
    public void Init()
    {
        //�s���p�ɾ� = �p�ɾ�.�q��ƫإ�(�s�u���澹�D�s���ɶ�)
        life = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    #endregion

}