/// 작성자: 고승로
/// 작성일: 2020-08-24
/// 수정일: 2020-09-04
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
      
/// < IVisualObject > 
/// Init     (한번): 프로그램 실행 시, 1회 실행됩니다.        
/// Active   (한번): 새로운 컷이 실행될 때, 1회 실행됩니다.  
/// MyUpdate (반복): IsFinish == true 가 될때까지 반복합니다.                     
/// 


using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNI
{
    /// <summary>
    /// 시퀀스 관리 관련 인터페이스
    /// </summary>
    public interface IVisualObject
    {
        /// <summary>
        /// Type에 따라 불러오는 오브젝트가 다릅니다.
        /// </summary>
        VisualType Type
        {
            get;
        }

        /// <summary>
        /// 해당 컷 종료유무 반환
        /// </summary>
        bool IsFinish
        {
            get;
        }

        /// <summary>
        /// 프로그램 실행 후 1회 실행
        /// </summary>
        void Init();

        /// <summary>
        /// IsFinish ==true 이면 다음 컷으로 넘어갑니다. false 면 MyUpdate 계속 진행
        /// </summary>
        void MyUpdate();

        /// <summary>
        /// 새로운 컷이 나올때 1회 실행
        /// </summary>
        /// <param name="option"></param>
        void Active(CutData option);

    }
}