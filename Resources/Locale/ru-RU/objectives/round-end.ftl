objectives-round-end-result =
    { $count ->
        [one] Был один.
        [few] Было { $count }.
       *[other] Было { $count }.
    }
objectives-round-end-result-in-custody = { $custody } из { $count } были арестованы.
objectives-player-user-named = [color=White][/color] ([color=gray][/color])
objectives-player-named = [color=White][/color]
objectives-no-objectives = { $custody }{ $title }.
objectives-with-objectives = { $custody }{ $title } со следующими целями:
objectives-objective-success = { $objective } | [color={ $markupColor }]Успех![/color]
objectives-objective-fail = { $objective } | [color={ $markupColor }]Провал![/color] ({ $progress }%)
objectives-in-custody = [bold][color=red]| АРЕСТОВАН | [/color][/bold]
