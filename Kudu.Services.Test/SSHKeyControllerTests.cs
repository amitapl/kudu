﻿using Kudu.Contracts.Infrastructure;
using Kudu.Contracts.Tracing;
using Kudu.Core.SSHKey;
using Kudu.Services.SSHKey;
using Moq;
using Xunit;

namespace Kudu.Services.Test
{
    public class SSHKeyControllerTests
    {
        [Fact]
        public void GetPublicKeyDoesNotForceRecreatePublicKeyByDefault()
        {
            // Arrange
            var sshKeyManager = new Mock<ISSHKeyManager>(MockBehavior.Strict);
            string expected = "public-key";
            sshKeyManager.Setup(s => s.GetKey()).Returns(expected).Verifiable();
            var tracer = Mock.Of<ITracer>();
            var operationLock = new Mock<IOperationLock>();
            operationLock.Setup(l => l.Lock()).Returns(true);
            var controller = new SSHKeyController(tracer, sshKeyManager.Object, operationLock.Object);

            // Act
            string actual = controller.GetPublicKey();

            // Assert
            sshKeyManager.Verify();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreatePublicKeyForcesRecreateIfParameterIsSet()
        {
            // Arrange
            var sshKeyManager = new Mock<ISSHKeyManager>(MockBehavior.Strict);
            string expected = "public-key";
            sshKeyManager.Setup(s => s.CreateKey()).Returns(expected).Verifiable();
            var tracer = Mock.Of<ITracer>();
            var operationLock = new Mock<IOperationLock>();
            operationLock.Setup(l => l.Lock()).Returns(true);
            var controller = new SSHKeyController(tracer, sshKeyManager.Object, operationLock.Object);

            // Act
            string actual = controller.GetPublicKey(forceCreate: true);

            // Assert
            sshKeyManager.Verify();
            Assert.Equal(expected, actual);
        }
    }
}