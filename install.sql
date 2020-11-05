INSERT INTO tc_modules (module_id, display_name, version, enabled, config_page, component_directory, security_class)
VALUES ('4911e2b1-49a5-4bcd-a768-a59419cf2fc7', 'Alexr03 Common', '2.0', 1, null, null, null);

-- ---------------------------------------------------------------------------------------------------------------------

create table ar_common_configurations
(
    id       int         not null,
    moduleId varchar(36) not null,
    name     text        not null,
    typeName text        null,
    contents text        not null,
    app_data text        not null,
    primary key (id, moduleId)
);

INSERT INTO ar_common_configurations (id, moduleId, name, typeName, contents, app_data)
VALUES (1, '4911e2b1-49a5-4bcd-a768-a59419cf2fc7', 'ArCommonSettings',
        'Alexr03.Common.ArCommonSettings, Alexr03.Common', '{}', '<?xml version="1.0" encoding="utf-16" standalone="yes"?>
<values>
  <add key="AR_COMMON:ConfigurationView" value="ArCommonConfiguration" type="System.String,mscorlib" />
</values>');

create table ar_common_sql_scripts
(
    id       int         not null,
    moduleId varchar(36) not null,
    name     text        null,
    contents text        null,
    app_data text        null,
    primary key (id, moduleId)
);

-- ---------------------------------------------------------------------------------------------------------------------

INSERT INTO tc_module_server_components (module_id, component_id, display_name, short_name, description,
                                         component_type, visible, component_class, required, startup_order)
VALUES ('4911e2b1-49a5-4bcd-a768-a59419cf2fc7', 1, 'Alexr03 Common Service', 'arcommon',
        'Alexr03 Common Service for interactivity between modules', 1, 1,
        'Alexr03.Common.TCAdmin.Services.ArCommonService, Alexr03.Common', 1, 100);

INSERT INTO tc_server_enabled_components (module_id, component_id, server_id)
VALUES ('4911e2b1-49a5-4bcd-a768-a59419cf2fc7', 1, 1);

-- ---------------------------------------------------------------------------------------------------------------------

INSERT INTO tc_panelbar_categories (category_id, module_id, display_name, view_order, parent_category_id,
                                    parent_module_id, page_id, panelbar_icon)
VALUES (1, '4911e2b1-49a5-4bcd-a768-a59419cf2fc7', 'Alexr03 Common', 900, 6, '07405876-e8c2-4b24-a774-4ef57f596384',
        null, null);

INSERT INTO tc_site_map (page_id, module_id, parent_page_id, parent_page_module_id, category_id, url, mvc_url,
                         controller, action, display_name, page_small_icon, panelbar_icon, show_in_sidebar,
                         view_order, required_permissions, menu_required_permissions, page_manager,
                         page_search_provider, cache_name)
VALUES (1, '4911e2b1-49a5-4bcd-a768-a59419cf2fc7', 40, '07405876-e8c2-4b24-a774-4ef57f596384', 1, '/ArCommon',
        '/ArCommon', 'ArCommon', 'Index', 'Alexr03.Common', 'MenuIcons/Base/ServerComponents24x24.png',
        'MenuIcons/Base/ServerComponents16x16.png', 1, 1000, '({07405876-e8c2-4b24-a774-4ef57f596384,0,8})',
        '({07405876-e8c2-4b24-a774-4ef57f596384,0,8})', null, null, null);