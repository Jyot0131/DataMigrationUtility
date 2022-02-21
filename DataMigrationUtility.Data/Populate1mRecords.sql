Declare @Id int
Set @Id = 1
While(@Id <= 1000000)
Begin
Insert into SourceTable values(RAND()*(1000000 - 1 + 1)+1,RAND()*(1000000-1+1)+1)
Set @Id = @Id + 1
End