using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ProtocolNexus.Tests
{
    public class GameTests
    {
        private GameObject playerObj;
        private PlayerStatus playerStatus;
        private PlayerController playerController;

        [SetUp]
        public void Setup()
        {
            // Instancia um player de teste básico
            playerObj = new GameObject("PlayerTest");
            playerObj.tag = "Player";
            
            var rb = playerObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // Evitar que caia infinitamente nos testes

            playerObj.AddComponent<BoxCollider2D>();
            
            playerController = playerObj.AddComponent<PlayerController>();
            playerStatus = playerObj.AddComponent<PlayerStatus>();
            playerStatus.vidasMaximas = 3;
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(playerObj);
        }

        [Test]
        public void Tatsuya_ComecaComVidaMaxima()
        {
            // Assegurar que ele não morreu na inicialização
            Assert.AreEqual(3, playerStatus.GetVidas());
        }

        [Test]
        public void Tatsuya_TomaDanoEPerdeVida()
        {
            // Act
            playerStatus.ReceberDano(1);

            // Assert
            Assert.AreEqual(2, playerStatus.GetVidas());
        }

        [Test]
        public void Tatsuya_MorreQuandoVidaChegaAZero()
        {
            // Act
            playerStatus.ReceberDano(3);

            // Assert
            Assert.AreEqual(0, playerStatus.GetVidas());
        }
        
        [UnityTest]
        public IEnumerator Tatsuya_MoveParaADireita_QuandoInputForcado()
        {
            var rb = playerObj.GetComponent<Rigidbody2D>();
            
            // Força velocidade
            rb.velocity = new Vector2(5f, 0f);
            
            yield return new WaitForFixedUpdate();
            
            Assert.Greater(rb.velocity.x, 0f);
            Assert.Greater(playerObj.transform.position.x, 0f); // Se moveu pra direita a partir de 0
        }
    }
}
