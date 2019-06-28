# BlazorBug04
Sample app demonstrating nested binding using `@bind-XXX` syntax does not work

![Demo of bug](BlazorBug04.gif?raw=true "Bug")

Logged as [#11679](https://github.com/aspnet/AspNetCore/issues/11679)

### Description

The form on the Home page has two controls inside an `EditForm` component.

The first is an `InputText` component which is bound using `@bind-Value`

The second is a custom component, `MyControl` which also uses the same binding.
Within `MyControl` we have a nested `InputText` component which again uses
`@bind-Value`.

As shown in the demo, the initial values are put in the `<input>` controls 
when initially created, but when the text boxes are changed, the
values only get propogated to the directly bound version.

Similar issues have been reported before but it does not seem to be resolved
e.g.

[My original issue #1490](https://github.com/aspnet/Blazor/issues/1490)
[#5504](https://github.com/aspnet/AspNetCore/issues/5504)

Was supposed to be fixed in [#6351](https://github.com/aspnet/AspNetCore/issues/6351)?

