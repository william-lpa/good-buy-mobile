using GoodBuy.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UITest1
{
    public class AzureServiceTest : BaseTest
    {
        public string NewGUID => Guid.NewGuid().ToString();

        [Test]
        public void InicialiarClient_CamposEssenciais_NaoDevemSerNulo()
        {
            Assert.NotNull(azureService);
            Assert.NotNull(azureService.Client);
            Assert.NotNull(azureService.Store);
            //Assert.NotNull(azureService.Tables);
        }

        [Test]
        public void GetTabelaMarca_NaoDeveDarErro_QuandoInseridoAlteradoExcluidoParaOServidor()
        {
            //init
            var marca = new Marca("Marca") { Id = NewGUID };
            //IMobileServiceSyncTable<Marca> localTable = null;
            //IMobileServiceTable<Marca> serverTable = azureService.Client.GetTable<Marca>();

            //Assert.DoesNotThrow(() => localTable = azureService.GetTable<Marca>());
            //Assert.DoesNotThrow(() => serverTable = azureService.Client.GetTable<Marca>());
            //Assert.NotNull(localTable);
            //Assert.NotNull(serverTable);

            ////insert
            //Assert.DoesNotThrowAsync(async () => await localTable.InsertAsync(marca));
            //Assert.DoesNotThrow(() => azureService.Client.SyncContext.PushAsync());
            ////assert
            //string marcaAtual = GetNameByIdMarca(localTable, marca).Result;
            //Assert.NotNull(marcaAtual);
            //Assert.AreEqual(marcaAtual, "Marca");
            //Assert.AreEqual(ReadMarcaServerTableAsync(serverTable, marca).Result, 1);

            ////update
            //marca.Nome = "Marcaaa2";
            //Assert.DoesNotThrowAsync(async () => await localTable.UpdateAsync(marca));
            //Assert.DoesNotThrow(() => azureService.Client.SyncContext.PushAsync());
            ////assert
            //marcaAtual = GetNameByIdMarca(localTable, marca).Result;
            //Assert.NotNull(marcaAtual);
            //Assert.AreEqual(marcaAtual, "Marcaaa2");
            //Assert.AreEqual(ReadMarcaServerTableAsync(serverTable, marca).Result, 1);

            ////delete
            //Assert.DoesNotThrowAsync(async () => await localTable.DeleteAsync(marca));
            //Assert.DoesNotThrowAsync(async () => await serverTable.DeleteAsync(marca));
            ////assert
            //marcaAtual = GetNameByIdMarca(localTable, marca).Result;
            //Assert.IsNull(marcaAtual);
            //Assert.AreEqual(ReadMarcaServerTableAsync(serverTable, marca).Result, 0);

        }

        private async Task<int> ReadMarcaServerTableAsync(IMobileServiceTable<Marca> serverTable, Marca marca)
        {
            var count = await serverTable.ReadAsync($"select id from Marca where id='{marca.Id}' and Marca.Nome='{marca.Nome}'");
            return count?.ToList()?.Count ?? 0;
        }
        private async Task<string> GetNameByIdMarca(IMobileServiceSyncTable<Marca> localTable, Marca marca) => (await localTable.LookupAsync(marca.Id))?.Nome;

        //[Test]
        //public void GetTabelaSabor_NaoDeveDarErro_QuandoInseridoAlteradoExcluidoParaOServidor()
        //{
        //    //init
        //    var sabor = new Sabor("Sabor") { Id = NewGUID };
        //    IMobileServiceSyncTable<Sabor> localTable = null;
        //    IMobileServiceTable<Sabor> serverTable = null;
        //    //assert
        //    Assert.DoesNotThrow(() => localTable = azureService.GetTable<Sabor>());
        //    Assert.DoesNotThrow(() => serverTable = azureService.Client.GetTable<Sabor>());
        //    Assert.NotNull(localTable);
        //    Assert.NotNull(serverTable);

        //    //insert            
        //    Assert.DoesNotThrowAsync(async () => await localTable.InsertAsync(sabor));
        //    Assert.DoesNotThrow(() => azureService.Client.SyncContext.PushAsync());
        //    //assert
        //    string saborAtual = GetNameByIdMarca(localTable, sabor).Result;
        //    Assert.NotNull(saborAtual);
        //    Assert.AreEqual(saborAtual, "Sabor");
        //    Assert.AreEqual(ReadSaborServerTableAsync(serverTable, sabor).Result, 1);

        //    //update
        //    sabor.Nome= "Saboor2";
        //    Assert.DoesNotThrowAsync(async () => await localTable.UpdateAsync(sabor));
        //    Assert.DoesNotThrow(() => azureService.Client.SyncContext.PushAsync());
        //    //assert
        //    saborAtual = GetNameByIdMarca(localTable, sabor).Result;
        //    Assert.NotNull(saborAtual);
        //    Assert.AreEqual(saborAtual, "Saboor");
        //    Assert.AreEqual(ReadSaborServerTableAsync(serverTable, sabor).Result, 1);

        //    //delete
        //    Assert.DoesNotThrowAsync(async () => await localTable.DeleteAsync(sabor));
        //    Assert.DoesNotThrowAsync(async () => await serverTable.DeleteAsync(sabor));
        //    //assert
        //    saborAtual = GetNameByIdMarca(localTable, sabor).Result;
        //    Assert.IsNull(saborAtual);
        //    Assert.AreEqual(ReadSaborServerTableAsync(serverTable, sabor).Result, 0);

        //}

        //private async Task<int> ReadSaborServerTableAsync(IMobileServiceTable<Sabor> serverTable, Sabor sabor)
        //{
        //    var count = await serverTable.ReadAsync($"select id from Sabor where id='{sabor.Id}' and Sabor.Nome='{sabor.Nome}'");
        //    return count?.ToList()?.Count ?? 0;
        //}
        //private async Task<string> GetNameByIdMarca(IMobileServiceSyncTable<Sabor> localTable, Sabor marca) => (await localTable.LookupAsync(marca.Id))?.Nome;

    }
}


