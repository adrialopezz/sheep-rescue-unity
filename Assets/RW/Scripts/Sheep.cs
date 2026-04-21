/*
 * Copyright (c) 2019 Razeware LLC
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish,
 * distribute, sublicense, create a derivative work, and/or sell copies of the
 * Software in any work that is designed, intended, or marketed for pedagogical or
 * instructional purposes related to programming, coding, application development,
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works,
 * or sale is expressly withheld.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;

public class Sheep : MonoBehaviour{
    public float runSpeed;
    public float gotHayDestroyDelay;
    public float dropDestroyDelay;
    public int pointsOnHit = 1;

    private Collider myCollider;
    private Rigidbody myRigidbody;
    private bool hitByHay;
    private SheepSpawner sheepSpawner;

    private void Start(){
        myCollider = GetComponent<Collider>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void Update(){
        if (GameManager.Instance != null && (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver)){
            return;
        }

        transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
    }

    public void SetSpawner(SheepSpawner spawner){
        sheepSpawner = spawner;
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Hay") && !hitByHay){
            Destroy(other.gameObject);
            HitByHay();
        }
        else if (other.CompareTag("DropSheep")){
            Drop();
        }
    }

    private void Drop(){
        myRigidbody.isKinematic = false;
        myCollider.isTrigger = false;

        if (GameManager.Instance != null){
            GameManager.Instance.OnSheepDropped();
        }

        sheepSpawner.RemoveSheepFromList(gameObject);
        Destroy(gameObject, dropDestroyDelay);
    }

    private void HitByHay()
    {
        hitByHay = true;
        runSpeed = 0;

        TweenScale tweenScale = gameObject.AddComponent<TweenScale>();
        tweenScale.targetScale = 0;
        tweenScale.timeToReachTarget = gotHayDestroyDelay;

        if (GameManager.Instance != null){
            GameManager.Instance.OnSheepSaved(pointsOnHit);
        }

        sheepSpawner.RemoveSheepFromList(gameObject);
        Destroy(gameObject, gotHayDestroyDelay);
    }
}