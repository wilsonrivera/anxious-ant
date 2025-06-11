namespace AnxiousAnt;

public class SlugifyStringExtensionsTests
{
    private const string LongInputString =
        """
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc et dictum urna, et maximus felis. Vivamus egestas tellus sed arcu viverra, nec dignissim velit dictum. Morbi diam sapien, placerat eget laoreet in, tempus vitae neque. Suspendisse erat dolor, pharetra nec dapibus sit amet, auctor sed lorem. Nunc at suscipit purus, in fermentum augue. Curabitur semper rutrum sapien, eget maximus nulla sollicitudin a. Etiam vel condimentum sapien. Mauris suscipit ac neque porta tempor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Vivamus ac feugiat tellus, ut faucibus velit. Nullam sit amet eros magna. Sed posuere at ex ac imperdiet. Sed ac congue magna.

        Morbi est nisi, gravida ac porttitor nec, efficitur pharetra leo. In arcu lorem, elementum vel egestas a, porta ut nibh. Ut blandit neque non lacinia luctus. Cras auctor vehicula neque vitae semper. Quisque tincidunt lorem non leo lacinia, eget imperdiet ipsum tempor. Donec leo est, viverra at nulla at, gravida vulputate leo. Duis vehicula felis nisi. Aenean nec tellus tellus. In eget mollis urna. Donec rhoncus est vitae tortor posuere egestas. Vestibulum vitae nibh eu felis accumsan egestas.

        Nulla condimentum elit et erat tincidunt porta. Donec augue mauris, molestie vel sapien a, porta placerat ipsum. Proin sed lorem gravida odio pretium mattis. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Fusce vulputate sem dolor, non finibus libero posuere at. Sed volutpat ullamcorper purus, at vestibulum tortor laoreet et. Vivamus in enim nec metus vehicula semper sit amet vitae dolor. Quisque sit amet vulputate turpis. Donec tincidunt vitae nulla sed malesuada. Vestibulum aliquet interdum sagittis. Vivamus pharetra lorem quis eros posuere ullamcorper.

        Etiam tempor dolor dolor, vitae venenatis elit imperdiet vel. Vivamus in ipsum mi. Nam molestie pulvinar elit eget venenatis. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Ut tincidunt, magna hendrerit posuere mattis, velit ante efficitur ipsum, non tincidunt quam lectus non ex. Nam efficitur ullamcorper hendrerit. Suspendisse potenti.

        Nam convallis fermentum metus quis condimentum. Pellentesque commodo sagittis est aliquet mattis. In mattis lorem id magna interdum mollis. Nulla pretium id arcu ac bibendum. Quisque nec auctor neque, vitae hendrerit est. Suspendisse ornare rhoncus porttitor. Nulla facilisi. Integer sit amet sem ante. Morbi id ex tortor. Aliquam congue congue velit et fringilla. Nulla mattis ex id nibh porttitor dignissim. Nulla tortor diam, iaculis sed mauris auctor, pellentesque pulvinar magna.

        Praesent eget sapien vitae urna consectetur euismod et at erat. Nunc accumsan in tellus at tempor. Sed fringilla tortor nisl, et ultrices sem porta at. Nam porta lobortis sapien, at pulvinar mauris pulvinar sed. Quisque vulputate ex et eros cursus, at varius arcu vehicula. Praesent massa enim, viverra ac ipsum ac, hendrerit porttitor diam. Phasellus tellus ex, suscipit eu placerat vitae, aliquam et sapien.

        Vivamus at enim placerat, convallis lacus in, scelerisque est. Nullam bibendum lacus eget finibus rhoncus. In malesuada accumsan enim, eget interdum sapien eleifend sit amet. Suspendisse potenti. Curabitur auctor fringilla quam tempus scelerisque. Nullam a ligula egestas, efficitur eros vel, viverra augue. Duis luctus vulputate magna non ultricies. Fusce finibus non urna nec vulputate. Phasellus id ante eu urna dignissim consequat. Duis at turpis nulla. Vestibulum placerat pulvinar risus sed congue. Etiam id tortor id erat malesuada imperdiet at et ante. Pellentesque nec dui quis mauris interdum aliquam. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Morbi aliquam facilisis sodales.

        Pellentesque ultricies diam sit amet sem sagittis, sed volutpat nibh fringilla. Nunc blandit porta mollis. Interdum et malesuada fames ac ante ipsum primis in faucibus. Praesent commodo ante augue, quis facilisis orci suscipit sit amet. Aenean auctor faucibus libero, ut vestibulum felis vehicula at. Nulla molestie ornare diam fermentum tristique. Proin eget consectetur nibh. Integer non bibendum arcu. Fusce lacinia tortor vel nisi sollicitudin, ac fermentum ligula ornare. Proin congue semper erat, vel lobortis turpis. Maecenas et interdum dolor. Suspendisse luctus blandit ipsum eu sodales. Nunc venenatis suscipit risus quis consequat. Quisque id ante feugiat, commodo lectus sit amet, consequat enim. Maecenas dictum lectus nec augue scelerisque, eget pharetra felis venenatis.

        Vestibulum orci ex, posuere eu risus a, mattis facilisis nunc. Sed at ante vel mi dapibus mattis at id sem. Donec eu felis rhoncus, aliquam lectus ut, iaculis erat. Aenean mollis et neque quis iaculis. Morbi viverra nulla ac metus laoreet, eu semper tortor lobortis. Cras finibus maximus augue sit amet finibus. Integer congue volutpat purus, ac pharetra enim dapibus at. Proin convallis augue velit, sed euismod mi molestie id. Ut sollicitudin elit non tellus vulputate, eget sagittis est feugiat. Suspendisse in euismod diam, ut congue tortor. Praesent bibendum condimentum eros, eu elementum justo elementum quis.

        Integer nulla ligula, auctor et erat in, maximus placerat nisi. Aliquam facilisis nisi a elit bibendum aliquet. Mauris non nulla sed libero lobortis sagittis. Integer id mauris eu dolor lobortis mattis. Suspendisse et fermentum ex. Suspendisse non dictum metus. Phasellus neque diam, rhoncus sed tellus vel, interdum consectetur lectus. Vivamus mollis vulputate eros a semper. Aliquam felis nisi, commodo non sodales id, tincidunt eget dolor. Sed aliquet aliquam erat, vitae pretium tellus congue a. Maecenas diam dui, semper consequat ipsum non, luctus venenatis nunc. Donec imperdiet tincidunt varius. Cras rutrum sapien lectus, et gravida mi hendrerit ac. Donec vitae eros metus. Donec venenatis a nibh in maximus. Sed venenatis ornare erat ut finibus.

        Quisque sodales tortor eu felis pharetra feugiat. Etiam lacinia ante nec semper dignissim. Duis a dolor sodales, laoreet orci ut, finibus nunc. Duis vel vehicula elit. Vestibulum porttitor condimentum aliquam. Nullam aliquam, turpis et faucibus sagittis, eros augue mattis augue, quis consequat sem urna et purus. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Praesent nec volutpat eros.

        Suspendisse risus libero, varius in convallis sit amet, iaculis eu ipsum. Phasellus convallis risus justo, nec ornare ipsum accumsan aliquet. Vivamus pretium, nisi vitae aliquam convallis, diam ipsum pulvinar metus, vitae blandit augue augue ac felis. In aliquet malesuada neque, ornare facilisis risus efficitur vitae. Integer dictum dictum nisl, ac congue orci tincidunt eget. Etiam euismod lacinia tristique. Nunc pharetra faucibus erat, in sagittis felis hendrerit non. Morbi risus dolor, maximus ac ultrices sit amet, convallis at velit. Vestibulum eu nibh ut dui fermentum porttitor vel non dui. Proin augue erat, porta ac velit a, dapibus vestibulum tortor. Morbi vulputate arcu nisl, sed porta turpis viverra ut. Praesent rutrum libero ut velit fringilla luctus vel vel nibh. Quisque a massa ipsum. Curabitur faucibus auctor nunc, non rhoncus ipsum luctus at. Donec ultrices nulla ut neque dapibus, vel porta lorem vestibulum. In dui magna, fermentum nec pellentesque sed, dapibus sit amet ex.

        Donec posuere, nulla id sodales semper, diam lectus consectetur mauris, vitae eleifend diam lectus sit amet magna. Curabitur sodales eu leo quis sodales. Sed libero est, congue sed magna a, iaculis rutrum dolor. In tristique dui velit, a aliquam ex mattis ut. Donec augue nisi, interdum eget metus id, scelerisque hendrerit sapien. Donec et vulputate dui. Ut ut justo ac nisi pretium commodo. Cras in enim dui.

        Nulla in lacinia metus, sagittis vestibulum justo. Morbi eget nibh in velit luctus ultrices. Phasellus ut rutrum tellus, posuere iaculis mauris. Pellentesque justo purus, aliquet quis pulvinar id, rhoncus quis justo. Vivamus in neque eros. Curabitur vehicula placerat sapien, ac pharetra risus lacinia quis. Aliquam erat volutpat. Donec ut ante auctor, egestas tellus in, dapibus lectus. Duis interdum auctor mauris, non aliquet nulla consequat vitae.

        Fusce eu nulla interdum, finibus diam id, facilisis neque. Mauris semper nisl sed justo pulvinar, at lacinia risus sollicitudin. Nam mi.
        """;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void ShouldReturnEmptyStringWhenProvidedNullOrEmpty(string? value)
    {
        // Assert
        value.Slugify().ShouldBeEmpty();
        value.AsSpan().Slugify().ShouldBeEmpty();
    }

    [Theory]
    [InlineData("Foo Bar", "foo-bar")]
    [InlineData("foo bar baz", "foo-bar-baz")]
    [InlineData("foo bar ", "foo-bar")]
    [InlineData("       foo bar", "foo-bar")]
    [InlineData("[foo] [bar]", "foo-bar")]
    [InlineData("Foo ÿ", "foo-y")]
    [InlineData("FooBar", "foo-bar")]
    [InlineData("fooBar", "foo-bar")]
    [InlineData("UNICORNS AND RAINBOWS", "unicorns-and-rainbows")]
    [InlineData("Foo & Bar", "foo-and-bar")]
    [InlineData("Hællæ, hva skjera?", "haellae-hva-skjera")]
    [InlineData("Foo Bar2", "foo-bar2")]
    [InlineData("I ♥ Dogs", "i-love-dogs")]
    [InlineData("Déjà Vu!", "deja-vu")]
    [InlineData("Déjà\u180EVu!", "deja-vu")]
    [InlineData("fooBar 123 $#%", "foo-bar-123")]
    [InlineData("foo🦄", "foo")]
    [InlineData("🦄🦄🦄", "")]
    [InlineData("foo&bar", "foo-and-bar")]
    [InlineData("foo360BAR", "foo360-bar")]
    [InlineData("FOO360", "foo-360")]
    [InlineData("FOOBar", "foo-bar")]
    [InlineData("APIs", "apis")]
    [InlineData("APISection", "api-section")]
    [InlineData("Util APIs", "util-apis")]
    [InlineData("E¢Ðƕtoy  mÚÄ´¨ss¨sïuy   !!!!!  Pingüiño", "ec-dhvtoy-m-ua-ss-siuy-pinguino")]
    [InlineData("QWE dfrewf# $%!! asd", "qwe-dfrewf-asd")]
    [InlineData("You can't have any pudding if you don't eat your meat!",
        "you-cant-have-any-pudding-if-you-dont-eat-your-meat")]
    [InlineData("El veloz murciélago hindú", "el-veloz-murcielago-hindu")]
    [InlineData("Médicos sin medicinas medican meditando", "medicos-sin-medicinas-medican-meditando")]
    [InlineData("Você está numa situação lamentável", "voce-esta-numa-situacao-lamentavel")]
    [InlineData("crème brûlée", "creme-brulee")]
    [InlineData("ä ö ü", "a-o-u")]
    public void Slugify(string input, string expected)
    {
        // Assert
        input.Slugify().ShouldBe(expected);
        input.AsSpan().Slugify().ShouldBe(expected);
    }

    [Theory]
    [InlineData("Conway\'s Law", "conways-law")]
    [InlineData("Conway\'s", "conways")]
    [InlineData("Don\'t Repeat Yourself", "dont-repeat-yourself")]
    [InlineData("my parents\' rules", "my-parents-rules")]
    [InlineData("it-s-hould-not-modify-t-his", "it-s-hould-not-modify-t-his")]
    public void PossessivesAndContractionsTest(string input, string expected)
    {
        // Assert
        input.Slugify().ShouldBe(expected);
        input.AsSpan().Slugify().ShouldBe(expected);
    }

    [Fact]
    public void ProvidingLongInputShouldThrow()
    {
        // Arrange
        Func<string> act = LongInputString.Slugify;

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }
}