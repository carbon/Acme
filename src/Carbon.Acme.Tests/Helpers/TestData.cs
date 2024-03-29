﻿using System.Security.Cryptography;

namespace Carbon.Acme.Tests;

public static class TestData
{
    public static RSA ConstructRSAFromPem(string pem)
    {
        RSA rsa = RSA.Create();

        rsa.ImportFromPem(pem);

        return rsa;
    }

    public static RSA GetPrivateKey() => ConstructRSAFromPem(PrivateRSA256KeyText);

    // 2048 bits (256 bits)
    public const string PrivateRSA256KeyText =
        """
        -----BEGIN RSA PRIVATE KEY-----
        MIIEogIBAAKCAQEA5ZNtvz8Zu8z3HMxe0cpb0m4e0PJe7Uw3P7ss1rYdnifXBkuL
        ZPqy1EOKWpROKH7cbUw7QTzC16q99ZCRW/gsWLZUV+SWOdq5moQnGiRclkN1S69i
        n2OkfzUghPmUvqoXvRAMKaSdLH+5I+ErTQQ7EhX4V22njgKL3Ia/P92ooD9YY2w/
        4qy5RBXJmmN4UM5fzJXM1N/y0m4taN0I8BzPQm98r/psmJfTZbAdwNCA5BpGMNx9
        EeFb2s0lCfgPOVTn6cHRqRxVvu/eJHqtiCS+Loa6SCKdsRb1ZDK1iwqriOuzEX8n
        jHfF7f1p/nJZQ3/DgIDVKOdp1JpDyNC3Q39rbQIDAQABAoIBAE+xc+F1qBs3NXj7
        wNaWpnVP4MarvCQn1u36rcADDBkAv3Xh94ugMz/YCkkTPmZ0U1RlqBQnYYO4M6Tx
        AGSqq8pH0NOJ+FQxB0kKIwDCKpnG5QrW4SdvUYUPDjDa6rWdGgx5a59xE1aFJ+G6
        omtj0pWepszCte1oGOD+rZkf+w9VBXbaJCGU6vV56GIdj+niGScq0b2dTKzM4C8d
        YI/0OUpYjMllx/uFZNCDYsVHjZI2iKxrct0lCW/e976bIVMfGdABe0IpZwcPW0/v
        mlE1zqyfIXKiBFaEn+nSLoU5JzywsO1Z3DZBhQbC+DEkdwr3ytamLS3IlHi+Vio6
        X0dUjUkCgYEA/sUNTQrl6CsTUuQ/PUUX0kXYey60Nk871NvefCAXwNBSmxrmS/mZ
        wobzbyFncwVFODgZZ9qeXLbNSioFasL34484STIANBOeXGVEb7+CezbngxJUUZFh
        CCIdia79d6EsozbMgdUrzljjHeSgE3HvildUYLOWzZUMgmU4iIBTCE8CgYEA5q87
        YPzmHaguCTMR9LMnYjvouMlyoBwDl0YP3EO2LuRrhm+Idx4Who1mYbEBirYHUmZz
        05sNXAh4fR8n6elW0Ua3xIOSDAODWjCyYPkhu+gvc916UYToMv9l2YTHaHUU5vYZ
        xb1xmri6nZ1QR2Z+zmPJ/szL5H+eJtZjkgPVZYMCgYAMAB4rRdskf3ruiZ/M9Ac2
        OWIEE2Qbtsm7sgudzykvRODPO4ojhANRHWg0EN+9lD75cfMIaEzTUy0CVE5jBAIH
        LwTYtck6qr3n3kFF/kBYK3LmQqPEH9PxPSIlDSAVVXszyO4DI5Znptoz20QLBm/h
        FVH9KBhvYJjxP3FVRK4kkwKBgAjd3WNp9RJYv4Q9AUithisBcH2ByLUcvoqW988Y
        jy0YPIXLTax+bJE5aCEDI7MIMhnZtEQC5q2N6cutYeRcwqiClleoUvnBPNA/pOuT
        R9eoIgLLFYOCn0YuiOSmdN+v+GC0PuNZ178BsLKUNFy6NM+MCFyR1Jhwy0xk2yjm
        mtIvAoGAUXpGUdDj5gkXD418tKI1YKiCqqU9JhI2DJCGDMRwE4sS8A91/JoueRlC
        FQI14s8c03U8tJ0yzJ+iIU+fFRzB1GgBWLasLNmoGoWPiqL7MnVlnpaqnosCZGBq
        nBIAt6vwU9/glhk/GLS4AcEFgH+XP6DcsqhNX6srRgl6KuvC6Ic=
        -----END RSA PRIVATE KEY-----
        """;

    // actual
    // root

    public const string CertificateChain =
        """
        -----BEGIN CERTIFICATE-----
        MIIF6DCCBNCgAwIBAgITAPo/YUx8DKg7Oa5/yqAfLmlABDANBgkqhkiG9w0BAQsF
        ADAiMSAwHgYDVQQDDBdGYWtlIExFIEludGVybWVkaWF0ZSBYMTAeFw0xODAxMDUw
        MTM5MjlaFw0xODA0MDUwMTM5MjlaMB0xGzAZBgNVBAMTEmFjbWV2Mi53dWxmLmV1
        Lm9yZzCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAOQrsKP0qXIRvqGV
        NVd1GWFm23nZOGlXh/T9vT4jcuzJmR2K7WEIzUwB+sL0mXZ+E6K/fY7fnmKq3ooV
        zDgrKYoJt4r1xIe0qhYaL5+vlrwXVE5d1JtApf+anMMSP8A4FzvPZpxX9plfpjOc
        Y8uLkMKshvoLuWLDh+Pa1EhGuZ6MhrJDozXu1G4WxTX6DmvV6HrgVKVE6tRRkekS
        tF2R0OaqOEJlQ/K9j4vs1atQMMeNuRsPKRDdNLMjAo5oe4VC0hE0kZvs8lC4ylPq
        W5tHAhGwxuqTT1kGUWh6LVZqx1FqY5BGVTpFGVrUh/FdwReMwkoT01OgymwB/i9q
        m/UEQlGqsENIF+EPQs2nzJrQuGBgwXN6teWg17w10jLxQb/m/oTVT9cWEVnhNQxT
        f30p9lVWr2B/r4x7YMGq/MSBCUxHtIz8MSjlxNH2Vkm1yqwZorA2Adkg59NdHvnu
        wVnrb9cXRjoAdSnm2dGaB4IApGikf2Db+fYfBAmLS5IsWid3H8aq98eqYPbJwduk
        jmsTInGHTGbfaFiEdFT1/8i5wSyTrrRKzcXgzTxVD/Q14TOmB251jHzGCn/pv8VQ
        2PIKvyPdN2VJpLG3vGX5OT3OGeYGKXRERWrh//Bttny/wMZTwR82F8+444wn+HF6
        Y+XtXECBy6J3q61SIvstjmVnGQytAgMBAAGjggIaMIICFjAOBgNVHQ8BAf8EBAMC
        BaAwHQYDVR0lBBYwFAYIKwYBBQUHAwEGCCsGAQUFBwMCMAwGA1UdEwEB/wQCMAAw
        HQYDVR0OBBYEFLpoDfF3cLWGL2d93KBEEZYUKUWkMB8GA1UdIwQYMBaAFMDMA0a5
        WCDMXHJw8+EuyyCm9Wg6MHcGCCsGAQUFBwEBBGswaTAyBggrBgEFBQcwAYYmaHR0
        cDovL29jc3Auc3RnLWludC14MS5sZXRzZW5jcnlwdC5vcmcwMwYIKwYBBQUHMAKG
        J2h0dHA6Ly9jZXJ0LnN0Zy1pbnQteDEubGV0c2VuY3J5cHQub3JnLzAdBgNVHREE
        FjAUghJhY21ldjIud3VsZi5ldS5vcmcwgf4GA1UdIASB9jCB8zAIBgZngQwBAgEw
        geYGCysGAQQBgt8TAQEBMIHWMCYGCCsGAQUFBwIBFhpodHRwOi8vY3BzLmxldHNl
        bmNyeXB0Lm9yZzCBqwYIKwYBBQUHAgIwgZ4MgZtUaGlzIENlcnRpZmljYXRlIG1h
        eSBvbmx5IGJlIHJlbGllZCB1cG9uIGJ5IFJlbHlpbmcgUGFydGllcyBhbmQgb25s
        eSBpbiBhY2NvcmRhbmNlIHdpdGggdGhlIENlcnRpZmljYXRlIFBvbGljeSBmb3Vu
        ZCBhdCBodHRwczovL2xldHNlbmNyeXB0Lm9yZy9yZXBvc2l0b3J5LzANBgkqhkiG
        9w0BAQsFAAOCAQEATVr4/N+jzys/4zDUVcoyvSUjOinpYsbs/MR0XQZwUEUuHEQ0
        w6Sy49LKXDfyNp50uC5TXrlg/ZU3rZwmLddLYx7Hq7I1tbBx6Hnn4LLI2mgSA4Lp
        yfbIljT3VdrTEiKFwKsKEhBurHZaQrB5tSsPfsNiDSI1PQaDKxenuJ7jLCJ5Ro1/
        cChoqTExa3YP/9SQilp1M5RCYLvEKHC7eiG6AxOM+u4jdxhLT7MD3dm7ASgCOEtP
        fqzNOd7v8rMjDlIJJ/rI0bj/YAJ2Y+TGyhl5GzpUyrgBiYskOMlFi+ryMYOYpGrb
        VCoYVR32WMFzNaFc16jd9Ie40A3miM9XpNmN6A==
        -----END CERTIFICATE-----

        -----BEGIN CERTIFICATE-----
        MIIEqzCCApOgAwIBAgIRAIvhKg5ZRO08VGQx8JdhT+UwDQYJKoZIhvcNAQELBQAw
        GjEYMBYGA1UEAwwPRmFrZSBMRSBSb290IFgxMB4XDTE2MDUyMzIyMDc1OVoXDTM2
        MDUyMzIyMDc1OVowIjEgMB4GA1UEAwwXRmFrZSBMRSBJbnRlcm1lZGlhdGUgWDEw
        ggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDtWKySDn7rWZc5ggjz3ZB0
        8jO4xti3uzINfD5sQ7Lj7hzetUT+wQob+iXSZkhnvx+IvdbXF5/yt8aWPpUKnPym
        oLxsYiI5gQBLxNDzIec0OIaflWqAr29m7J8+NNtApEN8nZFnf3bhehZW7AxmS1m0
        ZnSsdHw0Fw+bgixPg2MQ9k9oefFeqa+7Kqdlz5bbrUYV2volxhDFtnI4Mh8BiWCN
        xDH1Hizq+GKCcHsinDZWurCqder/afJBnQs+SBSL6MVApHt+d35zjBD92fO2Je56
        dhMfzCgOKXeJ340WhW3TjD1zqLZXeaCyUNRnfOmWZV8nEhtHOFbUCU7r/KkjMZO9
        AgMBAAGjgeMwgeAwDgYDVR0PAQH/BAQDAgGGMBIGA1UdEwEB/wQIMAYBAf8CAQAw
        HQYDVR0OBBYEFMDMA0a5WCDMXHJw8+EuyyCm9Wg6MHoGCCsGAQUFBwEBBG4wbDA0
        BggrBgEFBQcwAYYoaHR0cDovL29jc3Auc3RnLXJvb3QteDEubGV0c2VuY3J5cHQu
        b3JnLzA0BggrBgEFBQcwAoYoaHR0cDovL2NlcnQuc3RnLXJvb3QteDEubGV0c2Vu
        Y3J5cHQub3JnLzAfBgNVHSMEGDAWgBTBJnSkikSg5vogKNhcI5pFiBh54DANBgkq
        hkiG9w0BAQsFAAOCAgEABYSu4Il+fI0MYU42OTmEj+1HqQ5DvyAeyCA6sGuZdwjF
        UGeVOv3NnLyfofuUOjEbY5irFCDtnv+0ckukUZN9lz4Q2YjWGUpW4TTu3ieTsaC9
        AFvCSgNHJyWSVtWvB5XDxsqawl1KzHzzwr132bF2rtGtazSqVqK9E07sGHMCf+zp
        DQVDVVGtqZPHwX3KqUtefE621b8RI6VCl4oD30Olf8pjuzG4JKBFRFclzLRjo/h7
        IkkfjZ8wDa7faOjVXx6n+eUQ29cIMCzr8/rNWHS9pYGGQKJiY2xmVC9h12H99Xyf
        zWE9vb5zKP3MVG6neX1hSdo7PEAb9fqRhHkqVsqUvJlIRmvXvVKTwNCP3eCjRCCI
        PTAvjV+4ni786iXwwFYNz8l3PmPLCyQXWGohnJ8iBm+5nk7O2ynaPVW0U2W+pt2w
        SVuvdDM5zGv2f9ltNWUiYZHJ1mmO97jSY/6YfdOUH66iRtQtDkHBRdkNBsMbD+Em
        2TgBldtHNSJBfB3pm9FblgOcJ0FSWcUDWJ7vO0+NTXlgrRofRT6pVywzxVo6dND0
        WzYlTWeUVsO40xJqhgUQRER9YLOLxJ0O6C8i0xFxAMKOtSdodMB3RIwt7RFQ0uyt
        n5Z5MqkYhlMI3J1tPRTp1nEt9fyGspBOO05gi148Qasp+3N+svqKomoQglNoAxU=
        -----END CERTIFICATE-----

        """;
}
