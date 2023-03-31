using System.Text;
using System.Threading.Tasks.Sources;
using Malom.Model;
using Malom.Persistence;
using Moq;

namespace MalomTest
{
    [TestClass]
    public class MalomGameModelTest
    {

        private MalomGameModel _model = null!;
        private MalomTable _mockedTable = null!;
        private Mock<IMalomDataAccess> _mock = null!;
        private IMalomDataAccess _dataAccess = null!;

        [TestInitialize]

        public void Initialize()
        {
            _mockedTable = new MalomTable();
            
            _mockedTable.SetValue(1, Values.Player1);
            _mockedTable.SetValue(10, Values.Player2);
            _mockedTable.SetValue(22, Values.Player1);

            _mock = new Mock<IMalomDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>())).Returns(() => Task.FromResult(_mockedTable));

            _dataAccess = new MalomFileDataAccess();

            _model = new MalomGameModel(_dataAccess);

        }

        [TestMethod]
        public void MalomGameModelNewGameTest() 
        {
            _model.NewGame();

            Assert.AreEqual(0, _model.GameState);
            Assert.AreEqual(18, _model.Table.CurrentNumberOfPieces);
            Assert.AreEqual(9, _model.Table.Player1NumberOfPieces);
            Assert.AreEqual(9, _model.Table.Player2NumberOfPieces);

            bool isAllEmpty = true;

            for (int i = 0; i < 24; i++)
            {
                if (_model.Table.GetValue(i) != Values.Empty)
                {
                    isAllEmpty = false;
                }
            }

            Assert.AreEqual(true, isAllEmpty);
        }

        [TestMethod]
        public void MalomGameModelNotIsGameOverTest()
        {
            _model.NewGame();

            Assert.AreEqual(false, _model.IsGameOver());

            _model.Table.StepValue(2);
            _model.Table.StepValue(10);

            Assert.AreEqual(false, _model.IsGameOver());

        }


        
        [TestMethod]
        public async Task MalomGameModelGameOverTest()
        {

            await _model.LoadGameAsync("..\\..\\..\\..\\test1.stl");

            _model.MovePiece(15, 7);
            _model.Mill(16);

            Assert.AreEqual(true, _model.IsGameOver());

        }

        [TestMethod]
        public async Task MalomGameModelCanMoveTest()
        {

            await _model.LoadGameAsync("..\\..\\..\\..\\test1.stl");

            _model.MovePiece(15, 7);

            Assert.AreEqual(true, _model.CanMove());

            await _model.LoadGameAsync("..\\..\\..\\..\\test2.stl");

            Assert.AreEqual(false, _model.CanMove());
        }

        [TestMethod]

        public void MalomGameModelStepTest()
        {
            _model.NewGame();

            _model.Step(1);

            Assert.AreEqual(Values.Player1, _model.Table.GetValue(1));

            _model.Step(12);

            Assert.AreEqual(Values.Player2, _model.Table.GetValue(12));

            _model.Step(23);

            Assert.AreEqual(Values.Player1, _model.Table.GetValue(23));
            Assert.AreEqual(15, _model.Table.CurrentNumberOfPieces);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MalomGameModelStepExceptionTest()
        {
            _model.NewGame();

            _model.Step(1);
            _model.Step(1);
        }

        [TestMethod]

        public async Task ChangeGameStateTest()
        {
            await _model.LoadGameAsync("..\\..\\..\\..\\test3.stl");

            Assert.AreEqual(0, _model.GameState);

            _model.Step(2);

            Assert.AreEqual(1, _model.GameState);
            Assert.AreEqual(0, _model.Table.CurrentNumberOfPieces);

        }

        [TestMethod]

        public async Task MalomGameModelMillPlaceTest()
        {
            await _model.LoadGameAsync("..\\..\\..\\..\\test4.stl");

            Assert.AreEqual(false, _model.CheckMill(21));

            _model.Step(22);

            Assert.AreEqual(true, _model.CheckMill(22));

            Assert.AreEqual(Values.Player2, _model.Table.GetValue(1));

            _model.Mill(1);

            Assert.AreEqual(Values.Empty, _model.Table.GetValue(1));
            Assert.AreEqual(8, _model.Table.Player2NumberOfPieces);
        }

        [TestMethod]

        public async Task MalomGameModelRemFromMillTest()
        {
            await _model.LoadGameAsync("..\\..\\..\\..\\test5.stl");

            Assert.AreEqual(false, _model.CheckMill(21));

            _model.Step(23);

            Assert.AreEqual(true, _model.CheckMill(22));

            Assert.AreEqual(true, _model.CheckMill(0));
            Assert.AreEqual(true, _model.CheckMill(1));
            Assert.AreEqual(true, _model.CheckMill(2));

            _model.Mill(1);

            Assert.AreEqual(Values.Player1, _model.Table.GetValue(1));
            Assert.AreEqual(Values.Player1, _model.Table.CurrentPlayer);
            Assert.AreEqual(9, _model.Table.Player1NumberOfPieces);
        }

        [TestMethod]

        public async Task MalomGameModelRemWrongPiece()
        {
            await _model.LoadGameAsync("..\\..\\..\\..\\test5.stl");

            _model.Step(23);

            _model.Mill(23);


            Assert.AreEqual(Values.Player2, _model.Table.GetValue(23));
            Assert.AreEqual(Values.Player1, _model.Table.CurrentPlayer);

            _model.Mill(3);


            Assert.AreEqual(Values.Empty, _model.Table.GetValue(3));
            Assert.AreEqual(Values.Player1, _model.Table.CurrentPlayer);
        }

        [TestMethod]
        public async Task MalomGameModelMoveTest()
        {
            await _model.LoadGameAsync("..\\..\\..\\..\\test1.stl");

            Assert.AreEqual(1, _model.GameState);

            _model.MovePiece(15, 23);

            Assert.AreEqual(Values.Empty, _model.Table.GetValue(15)); 
            Assert.AreEqual(Values.Player1, _model.Table.GetValue(23)); 
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task MalomGameModelMoveToOccupiedTest()
        {
            await _model.LoadGameAsync("..\\..\\..\\..\\test1.stl");

            Assert.AreEqual(1, _model.GameState);

            _model.MovePiece(0, 1);

            Assert.AreEqual(Values.Player1, _model.Table.GetValue(0)); 
            Assert.AreEqual(Values.Player1, _model.Table.GetValue(1));
            
            Assert.AreEqual(Values.Player2, _model.Table.CurrentPlayer);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task MalomGameModelMoveToNonAdjacentTileTest()
        {
           // await _model.LoadGameAsync("D:\\Marci\\elte\\felev_3\\eva\\bead01\\Malom\\test1.stl");
            await _model.LoadGameAsync("..\\..\\..\\..\\test1.stl");

            Assert.AreEqual(1, _model.GameState);
                
            _model.MovePiece(0, 22);

            Assert.AreEqual(Values.Player1, _model.Table.GetValue(0)); 
            Assert.AreEqual(Values.Empty, _model.Table.GetValue(23));
            
            Assert.AreEqual(Values.Player2, _model.Table.CurrentPlayer);
        }


        [TestMethod]
        public async Task MalomGameModelLoadTest()
        {
            _model = new MalomGameModel(_mock.Object);

            _model.NewGame();

            await _model.LoadGameAsync(String.Empty);

            for (int i = 0; i < 24; i++)
            {
                Assert.AreEqual(_mockedTable.GetValue(i), _model.Table.GetValue(i));
            }

            _mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());


        }
    }

}