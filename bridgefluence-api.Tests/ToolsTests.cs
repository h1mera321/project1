using NUnit.Framework;

namespace bridgefluence_api.Tests;

public class ToolsTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GetsUserIdFromValidRequest()
    {
        var result = bridgefluence.Tools.UtilityMethods.ExtractUserId(
            "vk_access_token_settings=menu&vk_app_id=8100687&vk_are_notifications_enabled=1&vk_is_app_user=1&vk_is_favorite=1&vk_language=en&vk_platform=mobile_web&vk_ref=catalog_recent&vk_ts=1648642106&vk_user_id=711176750&sign=BttSX0dzXFUMMG3OUnuYHqG8cKu75zosjvSUrKt6Y_E");

        Assert.AreEqual(result, 711176750);
    }

    // [Test]
    // public void ReturnsNoUserIdFromLousyForgedRequest()
    // {
    //     var result = bridgefluence.Tools.UtilityMethods.ExtractUserId(
    //         "vk_access_token_settings=menu&vk_app_id=8100687&vk_are_notifications_enabled=1&vk_is_app_user=1&vk_is_favorite=1&vk_language=en&vk_platform=mobile_web&vk_ref=catalog_recent&vk_ts=1648642106&vk_user_id=711176750&sign=BttSX0dzXFUMMG3OUnuYHqG8cKu75zosjvSUr-HUETA");
    //
    //     Assert.AreEqual(result, null);
    // }
}