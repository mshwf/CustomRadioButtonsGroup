# CustomRadioButtonsGroup

The control has:

``ItemsSurce`` bindable property that you can pass it a list used by the control.
and to specify the displayed property path, specify the ``DisplayMemberPath`` with a string represents the property
name, and use ``SelectedValuePath`` to specify the property name to receive its value when selections changes

example:
````
            <local:RadioButtonsGroup x:Name="rb" ItemsSource="{Binding DevTypes}" SelectedValuePath="Id" DisplayMemberPath="Title"/>
````
