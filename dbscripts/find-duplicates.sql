select
    top (1000)
             [Id]
             ,[FileName]
             ,[FileExtension]
             ,[FilePath]
             ,[FileSize]
             ,[FileDescription]
             ,[PhotoTaken]
             ,[FileCreated]
             ,[Hash]
FROM [photo-organizer-db].[dbo].[PhotoFiles]


truncate table [photo-organizer-db].[dbo].[PhotoFiles]

select
--     [FileName]
     [FilePath]
--      ,[FileSize]
--      ,[PhotoTaken]
--      ,[FileCreated]
     ,[Hash]
from [photo-organizer-db].[dbo].[PhotoFiles]
where [Hash] in (
    select [Hash]
    from [photo-organizer-db].[dbo].[PhotoFiles]
    group by [Hash]
    having count(*) > 1
)
group by [Hash], [FilePath]


-- format selected rows as html table where each row is an img tag


select convert (varchar(max), '<table>') +
       convert (varchar(max),
               stuff(
                       (select convert (varchar(max), '<tr><td><img width=300 src="') +
                               convert (varchar(max), [FilePath]) +
                               convert (varchar(max), '" /></td></tr>')
                        from [photo-organizer-db].[dbo].[PhotoFiles]
                        where [Hash] in (
                            select [Hash]
                            from [photo-organizer-db].[dbo].[PhotoFiles]
                            group by [Hash]
                            having count(*) > 1
                        )
                        for xml path(''), type
                       ).value('.', 'varchar(max)'), 1, 0, ''
                   )
           ) + convert (varchar(max), '</table>')


