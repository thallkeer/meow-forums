Insert into Forums (Created, Title, [Description], ImageUrl)
values
	(GETDATE(),'C#','This is a C# language forum','/images/forum/csharp.png'),
	(GETDATE(),'Python','This is a Python language forum','/images/forum/py.png'),
	(GETDATE(),'Java','This is a Java language forum','/images/forum/java.png'),
	(GETDATE(),'Javascript','This is a Javascript language forum','/images/forum/js.png'),
	(GETDATE(),'Swift','This is a Swift language forum','/images/forum/swift.png'),
	(GETDATE(),'Go','This is a Go language forum','/images/forum/go.png')
	
select * from Forums;


INSERT INTO Posts(Content, Created, ForumId, Title, UserId)
VALUES ('Second CSharp post!',GETDATE(),2,'Another C# Post!','48ab4264-3a20-4496-933c-02921a0c47ca'),
	   ('
The program has a parent class (BaseList) and two children (ListChain and MasList). 
I would like to prescribe 2 Sort sorting methods
 (one for the sheet, the second for the array) so that the sorting method for the array is in
  the parent class and when you call it, not finding this method in the child, 
  the Sort method of the parent class was used, and the second method Sort for 
  the sheet was registered in the child class. How to arrange these methods?
   The Sort method itself is not necessary.',GETDATE(),2,'Class Inheritance - change base class method','48ab4264-3a20-4496-933c-02921a0c47ca'),
	('More content',GETDATE(),2,'How to linq','48ab4264-3a20-4496-933c-02921a0c47ca');

select * from Posts;

select * From PostReplies;

select * from Forums;