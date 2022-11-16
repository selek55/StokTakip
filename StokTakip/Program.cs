using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StokTakip.Data;
using StokTakip.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StokTakipContext>(o => o.UseInMemoryDatabase("StokTakipDb"));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.WebHost
    .UseKestrel(options =>
    {
        options.ListenAnyIP(5000);
        //options.ListenAnyIP(5001, configure => configure.UseHttps()); 
    })
    .UseUrls("http://*:5000");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Stocks/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Stocks}/{action=Index}/{id?}");

app.Start();

AddTestData();

OpenBrowser("http://localhost:5000/");

app.WaitForShutdown();


void OpenBrowser(string url)
{
    Console.WriteLine(url);

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        Process.Start("xdg-open", url);
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
        Process.Start("open", url);
    }
    else
    {
        // throw 
    }
}

void AddTestData()
{
    var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<StokTakipContext>();

    context.AddRange(new[]
    {
        new Stock
                    {
                        Code = "PL30",
                        Name = "Paletli Pompa",
                        BuyingPrice = 8000,
                        SellingPrice = 15000,
                        Quantity = 10,
                        Status = true,

                        ImageData = new Image
                        {
                            Length = 1,
                            ContentType = "jpg",
                            FileName = "Paletli-Pompa.jpg",
                            Data = Convert.FromBase64String("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")
                        }
                    },
        new Stock
                    {
                        Code = "PL20",
                        Name = "Acil Durum Valfi",
                        BuyingPrice = 3000,
                        SellingPrice = 6500,
                        Quantity = 100,
                        Status = true,
                        ImageData = new Image
                        {
                            Length = 1,
                            ContentType = "jpg",
                            FileName = "Acil-Durum-Valfi.jpg",
                            Data = Convert.FromBase64String("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")
                        }
                    },
        new Stock
                    {
                        Code = "PL15",
                        Name = "Döküm Gövdeli Pompa",
                        BuyingPrice = 600,
                        SellingPrice = 2000,
                        Quantity = 80,
                        Status = true,
                        ImageData = new Image
                        {
                            Length = 1,
                            ContentType = "jpg",
                            FileName = "Döküm-Gövdeli-Pompa.jpg",
                            Data = Convert.FromBase64String("/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBwgHBgkIBwgKCgkLDRYPDQwMDRsUFRAWIB0iIiAdHx8kKDQsJCYxJx8fLT0tMTU3Ojo6Iys/RD84QzQ5OjcBCgoKDQwNGg8PGjclHyU3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3N//AABEIAHgAxAMBIgACEQEDEQH/xAAcAAABBQEBAQAAAAAAAAAAAAAAAQQFBgcDAgj/xABIEAABAgQCBQgHBQYCCwAAAAABAgMABAUREiEGEzFBUQciYXGBkaGxFCMyM2JywUJSgrLRFVNjosLhJEQWFyU0Q1RkkpPS8P/EABkBAAMBAQEAAAAAAAAAAAAAAAACAwEEBf/EACQRAAICAQUAAgIDAAAAAAAAAAABAhEDBBITITFBUUJxIjJh/9oADAMBAAIRAxEAPwDcYIIIACCCCAAghLwsABBBCXgAWCEjyVpBN1JHbAAjz7TDanH3EtoTtUs2AiJmdKaKwDedSu37tJUD2gWiicq9Yn1z7NPlKfUnpVlAdU7KpVq3FKuLFSUm2G3EbYzF+quYiPRGQd5exuKB/EbeEAG4TfKNRmVhtu63CbJC3Eo+piOmeUSeQLs0RGG3t+klY8ExjSa3PNXCVM4CLFvUIwnrFocS2kDCba6Vcl1ffknMH8irpgA2WR0qn6qoIlarRJZw7EONOqX3KKb9kOpiZn2R/jtMZNk79TLstjuWpZjFpyvhS2nWXUzbaUnE29L6tSSbWuQbK2bo7MVtGEl1LbJ+zq5ZOztMAGpOVSmj/edM517j6OpI/ImGjtfTKXVRa9U15izUzKGYQe04VDsPfGdKrw+y9Nn5ChPkDHFyslZzbfV88wvyyjLCjWZDlAmQsoqNJUUA++YVa/ThXa3fF1ptQlqlJtzUovG0u46QRkQekGPlqr1JxxDSUNIa9aOcnFc9Bucx2RsHIcZhVPqRU8TKpeSltm+SF2uogbr3EAGoQQQkaAsEEEABBBBAAQQR4cWltJWtQSkC5JNgIAPcEVPSHTaTprNqelqdmCbBKn0tIHSVH6X7Irp0+raecqVpys80IWoHsUbpPaRFFik/gVyRdK9pNSaCyh2oTSUhbmrSE3Ub2J3dRhnLaa0qac1UqpbqynHZKSBbLeeuMhNKqtXqD4kA9OXVrSxrDzL9ByG3dlnD9GielzACk0ubQLWu2+3fuC7wz0jck99f50LypKtpp09pUZZrWM055+xFwhQxAcbb+yKjN8plYClejUZlIvzdcl5Jt0hSExAihaTWs5J1TtST5GOg0er6vakZ+2/Gmw8TFZaSEvzofFqeP3Gn+7JM6cVaptql6lLGXaWDnJOOJUrLZewsPxCKhVqTOS74ep9TW4lWYBmRrWzwJBztxBiaXo9PoN322WxxdnGUeGImOL1Nal8Kn6hIKQSAtLT+tWkHK4AAvbhe+UXxaeEPyslm1G/yO0imKtMSqm0Vh/1RWE69s43EcSUg87wPXF7c5OqfVpRuYcriFodRiafaaAuLZZlWY6Iz3SmmSy22zSp4zBAJVdhTdj+Im8R1Cm9YfRHhsvhSobDvEPLDve3xfokp0r9G+kFJmKHVH5CbU2tbZulxpWJLidyh18N2cRZi2VKQTNyxCRhcQbpPDiIrqqfMBJUlKVpv7SFAju294jkzaaWKVfBbHlU0NEgkqCQTluESEupagStsoIyAMcJNC0vOEoVYWBsL59kPw08r2WXSPkiDhN+IpuX2eII7CUmlbJdQ61AfWOiadNHaltPWqNWnyv4M5I/ZHTUqJgJ5+HCb5C8aFyWV5+jU6oyco2Jt7WekrSq+IIwpTfLcCP7xTfQH2306woUgjYkHzj1UHDRnG6lSnnpeeY906lZyvtFthvFoaaaW6SEeWPiNoc0h0pWvCzRiDYG4bKhY9N7Q4p9S0v1wMxSEOMnaCtDZHVnGXUXSeqPSjTsrUZlttabBIc9g70w4dqVTe99UJtXW8r9Y6o6PcuqIvPTo3FmbaW5qlHVvWuW1EX/v2Q5jB6dOzEnMh1LzgvtuomNY0Sr37XlFNvEelM2x/GNyo5dRpZYu/UWx5VMsEEJBHKVOE7Msyco7MzC8DLSSpajuAjC9MNJv23Um35151lhK7SrCM8AH2j09MXvlVqimpaVpbSra31r1vuj2R337oxWvuFmZYdw3GqwjPeFG/mI2e6GPcvTo0kIZMlTVpIvGj9QrExJzbbNRU2uQTjLX7wDMAkbQbb9xhtQ9K5R2dQanS2JmVVnq1C62+hKsrjglWziIr0nMuoZDqVqRrpZIUAojiPICI9nElAcTkpPCPSwyWTHFyXpw58fHllFfBdNJKhKCqS9Z0UZmqdOMG6kkpwLHygnbvGw9e1w3prX55lLoqjoCtoQhCbHfsERtGln6myTLNFxSQMViMr7IWQoz0npEKZN/4VudbLzJNiMY2jLjY+EdXHijXRy7pMcvVirv31tTnFX/AI6h5GGTgW6cTq1rPFaio+MW9GibI95NrPyoAjujRinozW5ML61gfSKb8UfBNs36UgNADIW7IUN5RfW6FS0/5YrPxLP6x3RS5BHsybA60X84OePwg42Z2lsqOEC9huF4r1bk3JCdRMISpAUdpG/jG2ol5dtQU2y2lQBAIQAQD1RXOUKmioUJTgHrJc4gRwP97GJzyKaqh4x29nGh0WVqlMlJ8TLgD6AVJAHNVvHYQRFW0p0c9Cq7jTBcDLqA604BxyIO7aO4iJnksni7TZynqVdTKw62OAO0dV037YsGlLqJeTaqBlW5jUnCUObAFW4dISO2E3OX9hqS8M6pMs9JocQ+kqurmqG+HxVbIpIPTlD2sVCrKoqppuhysvLnCoPNsgqsTkRt8ogBUKq4mzaakBwbAb8AIbco+GbWyWS0+rNLKzfZbZCliYAupopHFdwO+IF0z5F3kOpvtM3O4fDmmOSWHlm+KmjpCyvxuYOQNhOqDaVAvTcqgAZXeQfC94j6qimzbRbNSlcR+6FK8hDYSj2K+vaB4tySleOD6wjiFAYXKnOpy2JW20k9mMHwjHktVRqj2OdBJdo1aYpDril69pS5ReFSEl1AvbMXzHlGmSlDpMw2l9DbpbcSlaUqcIIBG/pjHJeYcpdWlKmJpDiZV5LhBexrIvzh3XjZZWpSbC3mVTbRDbysCgq4UlXPy7SR2RPHKXiGnFejpFEpaNkog/MSfMxHMTH7A0oafRZMuojEBsCFZHu29gh6uuSCP8wD1AmISuz0rUFNGWxFaAQSU2uDFNkpdSE3JeGw9UERmjM0Z2gyT6rlRbwqN96cj4iCPFkmm0dq7VmTad1ETumFQZuCGMLSRwASL+JMVWYZl5gFqbbK0BRUnCbFJ64lKsdfpbVXVbdc7n+O30iMd94v5jHbxxlHaxI5JY5bo+nCZ92spAQALJSPspAsBDVtPMA3Q6mc2j0w1ccS0gqVs3RaEVGKSJzk5Scn6ywaCz5kq2htRsh46s9uzxtFq0+bW3TJaqsAh+mzKHhb7t+cPLxjPGCqXm2ngdpBHZFzqlefn6dMS62mg282UkZk2Ii8YuatEW6fZdW3UutpcQbpWkKB6CLx4fKwptaAVBCrqSnMkW2jqve0Z7R6zPfsiVbTMrSltsN2FticuHRHZc/NL9uZdP4zFFgbV2K8iL0ZtlN8aygfEki0eDUZL/m2P/IIoReWr2lqV1qMeVKvtzhuBfZnIXpdXkEbZpB+XOGU/Wqa9KPMKWtQcQUmyDsItFQwg5hCe6PKQUpUncCbRvDFGb2RWilQco+kbxbTixtrbsTkbkKHl4xaKnWZicpz8sttvApJyCc8sx9Ipa/V15ojK6gO82iyAElQhcUY9pjTb6JOQqbZ0Rcp7xWl5bTjaObewV7J8fCKM1rnG8BWXiNtmFO2690WNv3IztlaKvMS6VzLoW4ltIOX2rxPNFRqhsbbOyWlt+y4tA+BtpryzgN/tvuEfHOD+nOGyWZYK50ytXQlu3jf6QpMgk2OsJ4KcH/qIiUPavRTm4phXWXV+do8lyTTsS2eqWB/MTHRKGVAFuRcX1axXlClD2YRTUpFssSAfzEwAiLnlSSgrEl+5H3koB7LGLjS3i9KS7ufrGEKitqVPIB1bLSE4bjnNIt32MT1FURT5W5zDRHcqKad1Ji5VcSTz3R6ZJC45YoVtXPEdjn0c6RqvJ9MD/R/ASeY+sDwP1ghryeBSqG4QP8AMK/KmCPAzJcjPQg3tRmdRbLWk9WSr2vSHMujGf1iJd96v5jFn0xknJTTOoqLag24rEhYGRxBKvO/cYrk2nC8SNis46YO6FkNX/Y7YYzacTC7brGJBYxJtDWYRZhzqiy8EBm5ATe+HCodAI/UGJ9nnMpuTmIgpYZpHwIN++Jxk2bT1RbC6J5FbQ3pAwyqkfdcULdt4fZQwpfunel0/SHl4tGX8USa7PYKSARvzhcXARwSSlIFhYdMLj+Exu4KOuKEKso56wbwe6EKwQbGDcbRAz2VXaI/eJ8xFjvz1dkVyZ59Uat+8HmIn8XOUYlB9saXiPDSvVdp8zFacKVzb2CScfOI/esO6JvWatk4jYAXPRvita1BUtT00pIJyCEBXiSInlkNBUP0qmUZpkGGfnwJ87Xjp6ROgZTUu0PgWf6Yi9bInY/NLPwYR9DHpJYVk1Izjx+JS1flAiW4eh6444c3aijsSpR8QIbLcYscVQfPyoSn+owgS4BdNJaSP4twf51QFT49lmms9aW/7xjZtDYu08qKddMrURY+sSPpFoo5tT5XK3qQbdecVmZmJ7VYBONHGcKUMrUMz0AARa2glptKEnJACR2RTD0zJ+DnFHto+shtrIfUiVenplLTCCpbig2kdJik57Y2TjG3RrHJ6zqNGGFKsC8tbmfC9h4AQRPSMqiSk2JVr2GW0oTlwEEeLOW6TZ3JUqM85VJItz8nPJSbPNlpR6Um48Ce6M8fsRnG5aZ0k1igTDDacUwj1jI4qG7tFx2xh0yQxLLm3kFTTRGNAUQpWYyGR/8ArxbHkio9iSi2+hupMMqgSiVWUpUo5eyLwS1QL7pSpsIGI2AVe3DPfDtQjsXhI4yyDiBtlhA7rxJ4sLV+AhqwmFm3cDCrbdkXj0iUu2e6dlL33KUo+MObw3ZGraQj7ozj0peWW2GToxo6lUeilWRw5dOXnHIPqGy46so8YzG7jKHHNG1Rv0COTigEnKOeIw1mnylJF90ZZtDSXOtqyB904ie+Jq6iFYASv7IG8xEUyyVuvryysD0xISTvpSn2zklISAek3jIgyOqTE9KyZCm8ajkq2Zz23iNZlp1A9WzKo6VICld4BMTBxpUUPJwlPA7Y9MkOnC2lS1fdTn5RKSt2UVojRLVMjOaw/Lf62gVTZlz3886R1X+piws0mpPe4pk2u/Blf6Q1nwumzfok+gy8zYK1TibKAIuLjdkIX+H2Hf0RKaM39p55X4xb8v1hV0qUbQVKQpdh94/rFwpGidarVMbqVMbYelXEktqDyLqtu6D12jn/AKv9MakdT6B6An7Tz8w2SB8IQo/SElkxIZRkymUqVbfn8bbQQ0wbnbmrdFkDZtb6RfaDyVeiSyW5mcS3Y3slONSjxJuIn2uTymp94+8s9FhE3qYrwbjbMplZN6amG5eWbU484oJSgDaY2DRDRVuhsodfwuTWHMjYknbaJKkaPU6jkrkmLOqFi4o3Vbh0CJUbIhlzufXwUjBILCCFgiA4hjMdPdHUyLr0+0zikHyS8lKMWBZ25cD59kafHh5pDzam3EJWhYwqSoXBHCEnBTVMaMnF2j5j9Ds5zEYOcDYbhnlDoC+yNZrPJrJzTqnqZMGVKjm0pOJHZvHjFfn+T2qyLQcaSmd4ol1BJH/dHowzQpI5nCRTUosnKGmUxMWSfVoNyePCLQnQzSWor1YpplGf4i0jvN8/CJuT5Nqg2gJUthH3ipdyTxyEUeoijFjZR8BhQ2Y0lnk3X/xZ1ofKkmHrPJ1Ji2tnHD8qAIm9UjeIyrVEwoYMatOaAMJZ/wBnPoDv/UoKk/ykW8Yp+kOjeksi2XPQZdbW8yCS5brBz8II6jcHHRVX8Mu3jcNhu6eqIWYdU+s2TtyAESC5CanXVNhiZdfOVghSiOoWix0/QSrzCMTkg62VDIEYSOmKcqj6xdl+FNUoNthCTe23rh/QNszfin6w3nKa/Jz0xKPjC6w6ptfWDaH9JbDYcA+G547YvjknJE5LovXJ/NyKKmZKflZZz0j3TrjSSUrH2bkbD5jpjVG2m2xZtCEjglNowJClIUFoJStJuCDmDujZdEa0mt0lD6j/AIhv1b6fiA29R2xy63FT3opgna2s56Y6USmi1KfnJhK3HEN4m2kJJxq2AE7s/rwj5gmKnMVOdnKrPuax957E4u1rkpI2cMxbgBH1NXtGaRpCB+1JXWqSnCFhakqA22uDFaPJDojixJlphOd7B6/mDHAdJl/I7puqhTvoE2pa6c+qzg26lVsnOrcezhH0Uw63MMNvsrC23EhaFDYoHMGKnK8mmjMstK/Rn3inYHZhZA7LxbWGm2GW2WUBDbaQlCUjJIAsAIAPVoWCCAAggggAIIIIACCCCAAhDBBAAWEEEEACwQQQAEJaCCAAhCMoIIAMr020DrE5WpypUtDMw0+oL1QcCFg2AO3LdxiqSlEq0s86y/TJtDmXN1Kjx4C0EEdWHPKLI5IKiUa0crT3u6ZNdakYfO0StM0Z0rlVqXKNKlSvJRL6BfuJggh562b6pCxwRNMo7c0zS5VufXjmktJDy8V7qtmbw8ggjiu+zoqgggggAIIIIACCCCAAggggA//Z")
                        },
                    },
        new Stock
                    {
                        Code = "PL08",
                        Name = "Kontrol Valfleri",
                        BuyingPrice = 1800,
                        SellingPrice = 3200,
                        Quantity = 100,
                        Status = true,
                        ImageData = new Image
                        {
                            Length = 1,
                            ContentType = "jpg",
                            FileName = "Kontrol-Valfleri.jpg",
                            Data = Convert.FromBase64String("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")
                        },
                    },
        new Stock
                    {
                        Code = "PL11",
                        Name = "Eksenel Valfleri",
                        BuyingPrice = 1000,
                        SellingPrice = 2000,
                        Quantity = 50,
                        Status = false,
                        ImageData = new Image
                        {
                            Length = 1,
                            ContentType = "jpg",
                            FileName = "Eksenel-Valfleri.jpg",
                            Data = Convert.FromBase64String("/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBwgHBgkIBwgKCgkLDRYPDQwMDRsUFRAWIB0iIiAdHx8kKDQsJCYxJx8fLT0tMTU3Ojo6Iys/RD84QzQ5OjcBCgoKDQwNGg8PGjclHyU3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3N//AABEIAFwAXAMBIgACEQEDEQH/xAAcAAEAAgMBAQEAAAAAAAAAAAAABgcEBQgDAgH/xAA6EAABBAIABAMEBQsFAAAAAAABAAIDBAURBhIhMQcTQTJRkbIUYXGhsSIjUnSBgqKzwdHhFTRTZHP/xAAUAQEAAAAAAAAAAAAAAAAAAAAA/8QAFBEBAAAAAAAAAAAAAAAAAAAAAP/aAAwDAQACEQMRAD8AvFERAREQEREBERAREQEREBERAREQEREBERAREQEWty2YrYyuZZXB3f8AJDgvduUoO3q7W6dD+db/AHQVzD4p3JXugrYhluXZLS2Ys2B19nR+O1hWfEbiDINnZRp1a5rtMjy15LmgHXXfTWz9SlEXh5hYJGStNsvj2WuE5YR0IPsa9CV+N4G4brk8mMY4kaPmPe7Y+vZQQJ/irxMHO19AHuHkHp/Epl4bceT8Q2Zsdl/Kbc15kDo28oe0d2633Hf6wfqX7lMBhKmLuOhxNJhbBIQ4QN2PyT6qrvBjbvEDHE/8cv8ALKDpJERARY16/Vx8Pm3JmxR71t3vWDd4mwtGSOOzka7DIQAecEDfQbPp+1Bs7EnkwSS62GNLiPsCqa94pZO8Xw4SnDDZjaZHRybkIYB7WzyjuftU34i4twVXFWw/Jwc7oHhgaS7Z5TodFzzHFDlOZ7nOdFFGe2wN/wCPs0gxX5Cx5kr5dP8AOdz8w7O2d/ivWxl7Np4kn5XuDQ0Ej0HZY+QtyWxX81jWeVC2NoDt7HoSsVpbr2h8Qg6y/wBYxzh+bstm/wDAGT5QViT32Tf7erdef1SRnzALeIgh+Uq5O9Rs16+Lna+WF7GulkiaASCOunE/coz4eeGGU4b4grZbI3qbvJY9vkwBztlzSPaIHv8AcrWRBGfEjI3MTwbkLuOndBZj8sRyNGyNyNB9D3BIVCWuKMxd0+zmLsg/RdO4D4b0rw8WiBwJfc7fK2SBx0O2pmLnC2d2H+gB0gluJoXcg7ns47JTwPYS2WOFx5+2tOPoevXaz4eF89O0AYPk7e09jQNdu52rJ4Wj1w5i2n0qRfIFIKsJJ7dEFVN4AzdrHPrOo0Ypnyc30iSwdtbrtpoPr6qFcTcMXOGL7MdZtMe6SITah3y9SW+uv0V09HA1rRtUh4yNB4v3rfJUjb97j/VBPPDThzEN4Qx1uTG1pLNiFrpZJWB5JA1sc29DQB0OimbaVRo02rAB7hGFqOA2eXwZhG/9KI/FoK3yAiIgIiIPC7UgvU56luJstedhjkjcOjmkaIVMXfBXJyW5fo2VpisDywmRji/kHbm0Nb0ruRBGcVg8jToVqr5ajfIiZHzAOfvlAG9dPctpDRus1zXIf3K+vxcVskQYf0OY+1kLP2BsYHyrAs8KYS5bdbv0I7lhwAL7JMnQduh6fct2iD4hijgiZDDG2OJjQ1jGDTWgdgB6BfaIgIiICIiAiIgIiICIiAiIg//Z")
                        },
                    }
    });

    context.SaveChanges();
}